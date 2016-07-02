namespace RoomManager.Model
{
	[TableAttribute("groups")]
	public class Group
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("leader_id")]
		public int LeaderId { get; set; }
		[ColumnAttribute("members")]
		public string Members { get; set; }
		[ColumnAttribute("reserve_date")]
		public double ReserverDate { get; set; }
		[ColumnAttribute("entry_date")]
		public double EntryDate { get; set; }
		[ColumnAttribute("checkout_date")]
		public double CheckoutDate { get; set; }
		[ColumnAttribute("status")]
		public CustomerStatus Status { get; set; }
	}
}

