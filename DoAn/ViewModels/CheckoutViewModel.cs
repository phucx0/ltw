using DoAn.Models.Booking;
using DoAn.Models.Payments;

namespace DoAn.ViewModels
{
    public class CheckoutViewModel
    {
        public Booking Booking { get; set; }
        public Payment Payment { get; set; }
        public string Url { get; set; }
    }
}
