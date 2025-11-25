using DoAn.Areas.Booking.Services;
using DoAn.Models.Accounts;
using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Data;
using DoAn.Models.Payments;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class BookingController : Controller
    {
        private readonly ModelContext _context;
        private readonly BookingService _bookingService;
        private readonly PaymentService _paymentService;
        public BookingController(BookingService bookingService, PaymentService paymentService,ModelContext context)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
            _context = context;
        }

        public async Task<IActionResult> Checkout(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var booking = await _context.Bookings
                .Where(b => b.BookingId == bookingId && b.UserId == userId && b.Status == "pending")
                .Select(b => new { b.BookingId, b.Status })
                .FirstOrDefaultAsync();

            if (booking == null)
                return RedirectToAction("Movies", "Movie", new { area = "" });

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
            if (fullBooking == null)
                return NotFound();
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null) return NotFound();
            Console.WriteLine($"Total amount: {fullBooking.TotalAmount}");
            decimal fakeAmount = 10000;
            string bank = "Mbbank";
            string qrLink = $"https://qr.sepay.vn/img?acc={_paymentService._accountNumber}&bank={bank}&amount={fakeAmount}&des=Booking{fullBooking.BookingId}";
            var viewModel = new CheckoutViewModel
            {
                Booking = fullBooking,
                Payment = payment,
                Url = qrLink
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Console.WriteLine("Nguoi dung chua xac thuc");
                return Unauthorized(new { message = "Chưa đăng nhập" });
            }
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var takenSeats = await _context.Tickets
                .Where(t => request.SeatIds.Contains(t.SeatId)
                         && (t.Status == "booked" || t.Status == "pending"))
                .Select(t => t.Seat)
                .ToListAsync();
            if(takenSeats.Count > 0)
            {
                string seatCodes = string.Join(", ", takenSeats.Select(s => $"{s.SeatRow}{s.SeatNumber}"));
                return Ok(
                    new { 
                        success = false,
                        message = $"Ghế {seatCodes} đã có khách hàng khác đặt!"
                    }
                );
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal basePrice = await _context.RoomTypes
                    .Where(rt => rt.Rooms.Any(r => r.Showtimes.Any(s => s.ShowtimeId == request.ShowtimeId)))
                    .Select(rt => rt.BasePrice)
                    .FirstOrDefaultAsync();

                decimal extraPrice = await _context.SeatTypes
                    .Where(st => st.Seats.Any(s => s.SeatId == request.SeatIds[0]))
                    .Select(st => st.ExtraPrice)
                    .FirstOrDefaultAsync();
                // Tổng tiền vé
                decimal totalAmount = (basePrice + extraPrice) * request.SeatIds.Count();
                // Tạo booking và payment record
                (Models.Booking.Booking booking, Payment payment) = await _bookingService.CreateBooking(userId, request.ShowtimeId, totalAmount);


                // Insert tickets
                bool ticketResult = await _bookingService.InsertTickets(request.SeatIds, booking.BookingId, userId, extraPrice, basePrice);
                if (!ticketResult)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Không thể thêm vé");
                }

                // Lưu thay đổi và commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(
                    new
                    {
                        success = true,
                        message = $"Tạo booking thành công!",
                        BookingId = booking.BookingId
                    }
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Tạo booking thất bại");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            using var transaction = await _context.Database.BeginTransactionAsync();
            // Cập nhật trạng thái 'canceled'
            await _context.Bookings
                .Where(b => b.BookingId == bookingId)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, "canceled"));
            await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, "canceled"));
            await _context.Tickets
                .Where(t => t.BookingId == bookingId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.Status, "canceled"));

            await transaction.CommitAsync();
            return RedirectToAction("Movies", "Movie", new { area = "" });
        }
    }
}
