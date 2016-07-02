using System;
using System.Runtime.Serialization.Formatters;

namespace RoomManager.Model
{
    public class LengthOutOfRangeException : System.Exception
    {
        public LengthOutOfRangeException() { }
        public LengthOutOfRangeException( string message ) : base( message ) { }
        public LengthOutOfRangeException( string message, System.Exception inner ) : base( message, inner ) { }
    }
    
    public class TableNameNotDefinedException : System.Exception
    {
        public TableNameNotDefinedException() { }
        public TableNameNotDefinedException( string message ) : base( message ) { }
        public TableNameNotDefinedException( string message, System.Exception inner ) : base( message, inner ) { }
    }

    public class InvalidConnectorException : System.Exception
    {
        public InvalidConnectorException() { }
        public InvalidConnectorException( string message ) : base( message ) { }
        public InvalidConnectorException( string message, System.Exception inner ) : base( message, inner ) { }
    }
    
    public class PrimaryKeyNotDefinedException : System.Exception
    {
        public PrimaryKeyNotDefinedException() { }
        public PrimaryKeyNotDefinedException( string message ) : base( message ) { }
        public PrimaryKeyNotDefinedException( string message, System.Exception inner ) : base( message, inner ) { }
    }

    public class NullKeyException : System.Exception
    {
        public NullKeyException() { }
        public NullKeyException( string message ) : base( message ) { }
        public NullKeyException( string message, System.Exception inner ) : base( message, inner ) { }
   }
}
