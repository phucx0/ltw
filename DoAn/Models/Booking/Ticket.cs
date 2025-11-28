using DoAn.Models.Accounts;
using DoAn.Models.Cinema;

namespace DoAn.Models.Booking;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public int? UserId { get; set; }

    public DateTime BookingTime { get; set; }

    public string? Status { get; set; }

    public decimal Price { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    //public virtual Showtime Showtime { get; set; } = null!;

    public virtual User User { get; set; }
    //public virtual ICollection<TicketCombo> TicketCombos { get; set; } = new List<TicketCombo>();
    public virtual Booking Booking { get; set; }
}
