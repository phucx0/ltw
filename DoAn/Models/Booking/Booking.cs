using DoAn.Models.Accounts;
using DoAn.Models.Payments;

namespace DoAn.Models.Booking
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ShowtimeId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime BookingTime { get; set; }
        public User User { get; set; }
        public Showtime Showtime { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}
