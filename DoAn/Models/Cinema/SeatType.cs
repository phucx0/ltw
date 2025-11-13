using System;
using System.Collections.Generic;

namespace DoAn.Models.Cinema;

public partial class SeatType
{
    public int TypeId { get; set; }

    public string? Name { get; set; }

    public decimal ExtraPrice { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
