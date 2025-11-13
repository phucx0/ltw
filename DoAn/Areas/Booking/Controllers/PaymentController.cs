using DoAn.Areas.Booking.Services;
using DoAn.Models.Accounts;
using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ModelContext _context;

        public PaymentController(PaymentService paymentService, ModelContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Failed()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> VerifyPayment(int bookingId)
        {
            // Kiểm tra đăng nhập
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Kiểm tra booking 
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                return NotFound("Không tìm thấy đơn đặt vé.");

            // Kiểm tra payment
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
            if (payment == null)
                return NotFound("Chưa có giao dịch thanh toán cho đơn này.");
            decimal fakeAmount = 10000;
            PaymentResult result = await _paymentService.VerifyPaymentAsync(payment.TransactionContent, fakeAmount);
            if (result.Success)
            {
                // Cập nhật trạng thái
                payment.Status = "paid";
                payment.Amount = result.Transaction?.AmountIn; // xác nhận lại số tiền thực tế
                payment.TransactionId = result.Transaction?.ReferenceNumber;
                booking.Status = "confirmed";
                await _context.SaveChangesAsync();

                // 
                return RedirectToAction("Success", "Payment", new { area = "Booking", bookingId });
            }
            else
            {
                // Cập nhật trạng thái thất bại nếu cần
                //payment.Status = "Failed";
                //booking.Status = "PaymentFailed";
                //await _context.SaveChangesAsync();

                return RedirectToAction("Failed", "Payment", new { area = "Booking", bookingId });
            }
        }
    }
}
