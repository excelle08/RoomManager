namespace RoomManager.Model
{
	[TableAttribute("groups")]
	public class Group
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("leader_id")]
		public int Leader_Id { get; set; }
		[ColumnAttribute("members")]
		public string Members { get; set; }
		[ColumnAttribute("reserve_date")]
		public double Reserve_Date { get; set; }
		[ColumnAttribute("entry_date")]
		public double Entry_Date { get; set; }
		[ColumnAttribute("checkout_date")]
		public double Checkout_Date { get; set; }
		[ColumnAttribute("status")]
		public CustomerStatus Status { get; set; }
	}
}

