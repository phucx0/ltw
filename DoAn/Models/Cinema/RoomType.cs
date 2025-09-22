namespace DoAn.Models.Cinema
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal BasePrice { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
