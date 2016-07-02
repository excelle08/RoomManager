using System;

namespace RoomManager.Model
{
    public enum ColumnConstraint
    {
        None = 0,
        PrimaryKey = 1,
        AutoIncrement = 2,
        Unique = 4,
        NotNull = 8
    }

    [AttributeUsageAttribute(AttributeTargets.Class, Inherited=false)]
    public class TableAttribute : System.Attribute
    {
        public string TableName { get; set; }
        public string Description { get; set; }
        public TableAttribute(string tablename, string desc = "") {
            TableName = tablename;
            Description = desc;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Property)]
    public class ColumnAttribute : System.Attribute
    {
        public ColumnConstraint Constraint { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public ColumnAttribute(string name, ColumnConstraint constraint = ColumnConstraint.None, string desc = "") {
            Constraint = constraint;
            Description = desc;
            Name = name;
        }
    }
}