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
using System.Threading.Tasks;

namespace DoAn.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ModelContext _context;
        public PaymentController(PaymentService paymentService, ModelContext context, IHubContext<PaymentHub> hub)
        {
            _paymentService = paymentService;
            _context = context;
        }

        public async Task<ActionResult> Success(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return View();
            }
            var movie = await _context.Movies
                .Include(m => m.Showtimes)
                .FirstOrDefaultAsync(m => m.Showtimes.Any(s => s.ShowtimeId == booking.ShowtimeId));
            if (movie == null)
            {
                return View();
            }
            var fullBooking = await _context.Bookings
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            //SuccessViewModel ViewModel = new SuccessViewModel
            //{
            //    Booking = booking,
            //    Movie = movie
            //};
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
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            // Deserialize từ JSON string
            var payload = JsonConvert.DeserializeObject<SepayPayload>(body);
            if (payload == null)
            {
                Console.WriteLine("Payload is null!");
                return new PaymentResult { Success = false, Message = "Payload null" };
            }
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => payload.Content.Contains(p.TransactionContent));
            if (payment == null)
            {
                Console.WriteLine("Không tìm thấy payment!");
                return new PaymentResult { Success = false, Message = "Không tìm thấy payment!" };
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);
            if (booking == null)
            {
                Console.WriteLine("Không tìm thấy booking");
                return new PaymentResult { Success = false, Message = "Không tìm thấy booking" };
            }

            // Đối chiếu số tiền thanh toán
            decimal fakeAmount = 10000;
            payment.Amount = payload.TransferAmount; // Phải cập nhật số tiền thực tế từ payload
            payment.TransactionId = payload.Id.ToString(); 
            if (fakeAmount == payload.TransferAmount)
            {
                // Cập nhật trạng thái
                payment.Status = "paid";
                booking.Status = "confirmed";

                await _context.Tickets
                    .Where(t => t.BookingId == booking.BookingId)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.Status, "booked"));

                await _context.SaveChangesAsync();
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
