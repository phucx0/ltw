using System;
using System.Collections.Generic;

namespace DoAn.Models.Booking;

public partial class TicketCombo
{
    public int TicketId { get; set; }

    public int ComboId { get; set; }

    public short? Quantity { get; set; }

    public virtual ComboItem Combo { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
