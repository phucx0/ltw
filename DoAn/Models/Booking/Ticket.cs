using DoAn.Models.Accounts;
using DoAn.Models.Cinema;
using DoAn.Models.Payments;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Booking;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int ShowtimeId { get; set; }

    public int SeatId { get; set; }

    public int? UserId { get; set; }

    public DateTime? BookingTime { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Seat Seat { get; set; } = null!;

    public virtual Showtime Showtime { get; set; } = null!;

    public virtual ICollection<TicketCombo> TicketCombos { get; set; } = new List<TicketCombo>();

    public virtual User? User { get; set; }
}
