using DoAn.Models.Booking;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Cinema;

public partial class Room
{
    public int RoomId { get; set; }

    public int BranchId { get; set; }

    public string? Name { get; set; }

    public short? Capacity { get; set; }

    public int? RoomTypeId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual RoomType? RoomType { get; set; }


    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
