using DoAn.Models.Accounts;

namespace DoAn.Models.Booking
{
    public class TicketPriceHistory
    {
        public int HistoryId { get; set; }
        public int ShowtimeId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedAt { get; set; }
        public int ChangedBy { get; set; }

        // Navigation properties
        public Showtime Showtime { get; set; }
        public User ChangedByUser { get; set; }
    }

}
