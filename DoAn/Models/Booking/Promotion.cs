using DoAn.Models.Payments;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Booking;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public string? DiscountType { get; set; }

    public decimal? Value { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
