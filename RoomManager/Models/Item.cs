namespace RoomManager.Model
{
	[TableAttribute("items")]
	public class Item
	{
		[ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
		public int Id { get; set; }
		[ColumnAttribute("name")]
		public string Name { get; set; }
		[ColumnAttribute("typical_price")]
		public float TypicalPrice { get; set; }
		[ColumnAttribute("description")]
		public string Description { get; set; }
	}
}

