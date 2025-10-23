using DoAn.Models.Booking;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Cinema;

public partial class Seat
{
    public int SeatId { get; set; }

    public int RoomId { get; set; }

    public int? TypeId { get; set; }

    public string? SeatRow { get; set; }

    public int? SeatNumber { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual SeatType SeatType { get; set; }
}
