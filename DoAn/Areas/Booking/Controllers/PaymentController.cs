using DoAn.Areas.Booking.Services;
using DoAn.Models.Accounts;
using DoAn.Models.Booking;
using DoAn.Models.Data;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace DoAn.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class PaymentController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        private readonly PaymentService _paymentService;
        private readonly BookingService _bookingService;

        [ActivatorUtilitiesConstructor]
        public PaymentController( PaymentService paymentService, BookingService bookingService, IDbContextFactory dbFactory)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
            _dbFactory = dbFactory;
        }

        public async Task<ActionResult> Success(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var booking = await db.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return View();
            }
            var movie = await db.Movies
                .Include(m => m.Showtimes)
                .FirstOrDefaultAsync(m => m.Showtimes.Any(s => s.ShowtimeId == booking.ShowtimeId));
            if (movie == null)
            {
                return View();
            }
            var fullBooking = await db.Bookings
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            return View();
        }
        public ActionResult Failed()
        {
            return View();
        }
        
        // Sepay sẽ gọi tới API này
        [HttpPost] 
        public async Task<PaymentResult> Callback()
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            // Deserialize từ JSON string
            var payload = JsonConvert.DeserializeObject<SepayPayload>(body);
            if (payload == null)
            {
                Console.WriteLine("Payload is null!");
                return new PaymentResult { Success = false, Message = "Payload null" };
            }

            int bookingId = 0;
            if (payload.Content.StartsWith("Booking") &&
                int.TryParse(payload.Content.Substring("Booking".Length), out var id))
            {
                bookingId = id;
            }
            else
            {
                return new PaymentResult { Success = false, Message = "Invalid booking content" };
            }
            Console.WriteLine(bookingId);
            var payment = await db.Payments.FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null)
            {
                Console.WriteLine("Khong tim thay payment!");
                return new PaymentResult { Success = false, Message = "Không tìm thấy payment!" };
            }

            var booking = await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                Console.WriteLine("Không tìm thấy booking");
                return new PaymentResult { Success = false, Message = "Không tìm thấy booking" };
            }

            decimal amountIn = 0;
            var transactionDetails = _paymentService.GetTransactionDetailsAsync(payload.Id);
            if (transactionDetails == null || transactionDetails.Result == null)
            {
                Console.WriteLine("Khong tim thay chi tiet giao dich!");
                return new PaymentResult { Success = false, Message = "Khong tim thay chi tiet giao dich!" };
            }
            payment.Amount = amountIn = decimal.Parse(transactionDetails.Result.AmountIn);
            payment.TransactionId = payload.Id.ToString();
            
            // Đối chiếu số tiền thanh toán
            decimal fakeAmount = 10000;

            //if (payment.Amount == amountIn) 
            if (fakeAmount == payload.TransferAmount)
            {
                // Cập nhật trạng thái
                payment.Status = "paid";
                booking.Status = "confirmed";

                await _bookingService.InsertTickets(booking.BookingId, booking.UserId);
                await db.SaveChangesAsync();
                await _paymentService.NotifyPaymentResult(booking.UserId, booking.BookingId, true);

                Console.WriteLine("Thanh toan thanh cong");
                return new PaymentResult { Success = true };
            }
            await _paymentService.NotifyPaymentResult(booking.UserId, booking.BookingId, false);

            Console.WriteLine("Thanh toan that bai");
            return new PaymentResult { Success = false };
        }
    }
}
