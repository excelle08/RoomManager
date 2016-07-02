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
		[ColumnAttribute("name")]
		public string Name { get; set; }
		[ColumnAttribute("password")]
		public string Password { get; set; }
		[ColumnAttribute("privilege")]
		public UserPrivilege Privilege { get; set; }
	}
}
