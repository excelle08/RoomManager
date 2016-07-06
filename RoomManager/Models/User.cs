namespace RoomManager.Model
{
	public enum UserPrivilege
    {
        Blocked,
        Staff,
        Admin
    };

	[TableAttribute("users")]
	public class User
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("username")]
		public string UserName { get; set; }
		[ColumnAttribute("password")]
		public string Password { get; set; }
		[ColumnAttribute("privilege")]
		public UserPrivilege Privilege { get; set; }
	}

	public class UserCredential
	{
		public string username { get; set; }
		public string password { get; set; }
		public bool remember { get; set; }
	}
}
