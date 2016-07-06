using System.Collections.Generic;
using System.Reflection;
using Dapper;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using SqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Diagnostics;
using System;

namespace RoomManager.Model
{
    public class DataHelper<T> : IDataHelper<T>
    {
        private SqlConnection connector;
        private string tablename;
        public DataHelper(ref SqlConnection conn) {
            connector = conn;
            
            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (attr == null) {
                throw new TableNameNotDefinedException("Table name is not specified for type: " + typeof(T).FullName);
            } else {
                tablename = attr.TableName;
            }
        }

        public T Insert(T item) {
            // Compose SQL command
            string fields = "", values = "";

            fields = String.Join(", ", GetColumnNames(false, true));
            values = String.Join(", ", GetValues(item, false, true));
            string sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2});", tablename, fields, values);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connector;
            cmd.Connection.Open();
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
            SetLastInsertId(ref item, (int)cmd.LastInsertedId);

            cmd.Connection.Close();

            return item;
        }

        public IEnumerable<T> SelectAll(string condition) {
            string sql = String.Format("SELECT * FROM {0};", tablename);
            IEnumerable<T> result;
            connector.Open();
            try {
                result = connector.Query<T>(sql);
            } catch (InvalidOperationException e) {
                Debug.Write(e.Message);
                return new List<T>();
            } finally {
                connector.Close();
            }

            return result;
        }

        public IEnumerable<T> Select(string condition, int offset = 0, int limit = 0) {
            string sql = "SELECT * FROM " + tablename;
            IEnumerable<T> result;
            if (condition.Length > 0) {
                sql += " WHERE " + condition;
            }
            if (offset > 0) {
                sql += " OFFSET " + offset.ToString();
            }
            if (limit > 0) {
                sql += " LIMIT " + limit.ToString();
            }

            connector.Open();
            try {
                result = connector.Query<T>(sql);
            } catch (InvalidOperationException e ) {
                Debug.Write(e.Message);
                return new List<T>();
            } finally {
                connector.Close();
            }

            return result;
        }

        public T SelectOne(string condition) {
            string sql = "SELECT * FROM " + tablename;
            T result;
            if (condition.Length > 0) {
                sql += " WHERE " + condition;
            }

            connector.Open();
            try {
                result = connector.QueryFirst<T>(sql);
            } catch ( InvalidOperationException e ) {
                Debug.Write(e.Message);
                return default(T);
            } finally {
                connector.Close();
            }

            return result;
        }

        public int Count(string condition = "") {
            string sql = "SELECT count(*) FROM " + tablename;
            if (condition != "") {
                sql += " WHERE " + condition;
            }
            int count;

            connector.Open();
            count = connector.QueryFirst<int>(sql);

            return count;
        }

        public int Update(T item) {
            string[] pkArr = GetPrimaryKey();
            string pk = pkArr[0], pkProp = pkArr[1];
            var pkvalue = item.GetType().GetProperty(pkProp).GetValue(item);
            if (pkvalue == null) {
                throw new NullKeyException("Cannot update an item whose key is empty: " + typeof(T).FullName);
            }
            string[] fields = GetColumnNames(true), values = GetValues(item, true);
            List<string> setvalues = new List<string>();
            for(int i = 0; i < fields.Length; i ++) {
                setvalues.Add(String.Format("{0}={1}", fields[i], values[i]));
            }

            string sql = String.Format("UPDATE {0} SET {1} WHERE {2}={3} ", 
                tablename, String.Join(", ", setvalues.ToArray()), pk, pkvalue);

            connector.Open();
            int retn = connector.Execute(sql);
            connector.Close();

            return retn;
        }

        public int Delete(T item) {
            string[] pkArr = GetPrimaryKey();
            string pk = pkArr[0], pkProp = pkArr[1];
            var pkvalue = item.GetType().GetProperty(pkProp).GetValue(item);
            if (pkvalue == null) {
                throw new NullKeyException("Cannot delete an item whose key is empty: " + typeof(T).FullName);
            }

            string sql = String.Format("DELETE FROM {0} WHERE {1}={2}", tablename, pk, pkvalue);
            connector.Open();
            int retn = connector.Execute(sql);
            connector.Close();

            return retn;
        }
        
        private string[] GetPrimaryKey() {
            string pk = "", pkProp = "";

            Type t = typeof(T);
            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                ColumnAttribute attr = prop.GetCustomAttribute<ColumnAttribute>();
                if ((attr.Constraint & ColumnConstraint.PrimaryKey) > 0) {
                    pk = attr.Name;
                    pkProp = prop.Name;
                }
            }

            if (pk == "") {
                throw new PrimaryKeyNotDefinedException("Primary key is not defined in type: " + typeof(T).FullName);
            } else {
                return new string[] {pk, pkProp};
            }
        }

        private string[] GetColumnNames(bool IgnorePK = false, bool IgnoreAutoIncrement = false) {
            List<string> names = new List<string>();

            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                ColumnAttribute attr = prop.GetCustomAttribute<ColumnAttribute>();

                if(attr == null) {
                    continue;
                }
                if(IgnorePK) {
                    if ((attr.Constraint & ColumnConstraint.PrimaryKey) > 0) {
                        continue;
                    }
                }
                if(IgnoreAutoIncrement) {
                    if ((attr.Constraint & ColumnConstraint.AutoIncrement) > 0) {
                        continue;
                    }
                }
                names.Add(attr.Name);
            }

            return names.ToArray();
        }

        private string[] GetValues(T item, bool IgnorePK = false, bool IgnoreAutoIncrement = false) {
            List<string> values = new List<string>();

            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if(IgnorePK || IgnoreAutoIncrement) {
                    ColumnAttribute fieldattr = prop.GetCustomAttribute<ColumnAttribute>();
                    if (fieldattr == null || IgnorePK && (fieldattr.Constraint & ColumnConstraint.PrimaryKey) > 0
                        || IgnoreAutoIncrement && (fieldattr.Constraint & ColumnConstraint.AutoIncrement) > 0) {
                        continue;
                    }
                }
                
                var fvalue = item.GetType().GetProperty(prop.Name).GetValue(item);
                if (fvalue is string) {
                    values.Add(String.Format("\"{0}\"", fvalue));
                } else if (fvalue is Enum) {
                    values.Add(((int) fvalue).ToString());
                } else {
                    values.Add(fvalue.ToString());
                }
            }
            return values.ToArray();
        }

        private void SetLastInsertId(ref T item, int value) {
            foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                ColumnAttribute fieldattr = prop.GetCustomAttribute<ColumnAttribute>();
                if ( fieldattr != null && (fieldattr.Constraint & ColumnConstraint.AutoIncrement) > 0) {
                    item.GetType().GetProperty(prop.Name).SetValue(item, value);
                }
            }
        }
    }
}