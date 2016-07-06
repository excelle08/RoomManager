namespace RoomManager.Model
{
	[TableAttribute("consumptions")]
	public class Consumption
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("customer")]
		public int Customer { get; set; }
		[ColumnAttribute("item")]
		public int Item { get; set; }
		[ColumnAttribute("count")]
		public int Count { get; set; }
		[ColumnAttribute("price")]
		public float Price { get; set; }
		[ColumnAttribute("paid")]
		public bool Paid { get; set; }
		[ColumnAttribute("comment")]
		public string Comment { get; set; }
	}
}
