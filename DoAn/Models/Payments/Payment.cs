using DoAn.Models.Booking;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Payments;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int? PromotionId { get; set; }

    public decimal Amount { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public string? TransactionId { get; set; }
    public string TransactionContent { get; set; }

    public DateTime? PaymentTime { get; set; }

    public virtual Promotion? Promotion { get; set; }
    public virtual Booking.Booking Booking { get; set; }
}
