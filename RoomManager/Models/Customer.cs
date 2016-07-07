namespace RoomManager.Model
{

    public enum CustomerStatus
    {
        InReservation,
        CheckedIn,
        CheckedOut
    }

	[TableAttribute("customers")]
	public class Customer
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("name")]
		public string Name { get; set; }
		[ColumnAttribute("gender")]
		public bool Gender { get; set; }
		[ColumnAttribute("identity")]
		public string Identity { get; set; }
		[ColumnAttribute("reserve_date")]
		public double Reserve_Date { get; set; }
		[ColumnAttribute("entry_date")]
		public double Entry_Date { get; set; }
		[ColumnAttribute("checkout_date")]
		public double Checkout_Date { get; set; }
		[ColumnAttribute("status")]
		public CustomerStatus Status { get; set; }
		[ColumnAttribute("room_id")]
		public int Room_Id { get; set; }
	}
}
