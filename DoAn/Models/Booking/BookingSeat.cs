using DoAn.Models.Cinema;

namespace DoAn.Models.Booking
{
    public class BookingSeat
    {
        public int BookingSeatId { get; set; }

        public int BookingId { get; set; }
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }

        public decimal Price { get; set; }

        // Navigation
        public virtual Booking Booking { get; set; }
        public virtual Showtime Showtime { get; set; }
        public virtual Seat Seat { get; set; }
    }

}
