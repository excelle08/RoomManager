using Dapper;
using MySql.Data;
using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using System.Collections;


namespace RoomManager.Model
{
    public class DBConnector
    {
        public static string ConnectionString;

        public static SqlConnection GetConnection(string server="127.0.0.1", string database="room_manager", string username="root", string password="", string charset="utf8")
        {
            ConnectionString = string.Format("server={0};database={1};uid={2};pwd={3};charset={4}", server, database, username, password, charset);

            return new SqlConnection(ConnectionString);
        }
    }
}

