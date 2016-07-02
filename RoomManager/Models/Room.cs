namespace RoomManager.Model
{
    public enum RoomStatus
    {
        Vacant,
        Reserved,
        CheckedIn,
        InMaintenance
    };

    [TableAttribute("rooms")]
    public class Room
    {
        [ColumnAttribute("id", ColumnConstraint.PrimaryKey | ColumnConstraint.AutoIncrement)]
        public int Id { get; set; }
        [ColumnAttribute("name")]
        public string Name { get; set; }
        [ColumnAttribute("number")]
        public int Number { get; set; }
        [ColumnAttribute("type")]
        public int Type { get; set; }
        [ColumnAttribute("custom_price")]
        public float CustomPrice { get; set; }
        [ColumnAttribute("capacity")]
        public int Capacity { get; set; }
        [ColumnAttribute("status")]
        public RoomStatus Status { get; set; }
        [ColumnAttribute("description")]
        public string Description { get; set; }
        
    }
}