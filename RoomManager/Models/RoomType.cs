namespace RoomManager.Model
{
    [TableAttribute("room_type")]
    public class RoomType
    {
        [ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
        public int Id { get; set; }
        [ColumnAttribute("name")]
        public string Name { get; set; }
        [ColumnAttribute("typical_price")]
        public float Typical_Price { get; set; }
    }
}