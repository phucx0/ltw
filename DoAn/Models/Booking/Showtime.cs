using DoAn.Models.Cinema;
using DoAn.Models.Movies;
using DoAn.Models.Payments;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Booking;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public int RoomId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? BasePrice { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;

    //public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

}
