using System;
using System.Collections.Generic;

namespace DoAn.Models.Booking;

public partial class ComboItem
{
    public int ComboId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<TicketCombo> TicketCombos { get; set; } = new List<TicketCombo>();
}
