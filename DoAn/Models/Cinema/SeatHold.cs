using DoAn.Models.Accounts;
using DoAn.Models.Booking;

namespace DoAn.Models.Cinema
{
    public class SeatHold
    {
        public int HoldId { get; set; }

        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }

        public DateTime ExpireAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual Showtime Showtime { get; set; }
        public virtual Seat Seat { get; set; }
    }

}
