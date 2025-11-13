using DoAn.Areas.Booking.Services;
using DoAn.Models.Booking;
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
            var booking = await _context.Bookings
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Room)
                        .ThenInclude(r => r.Branch)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
                return NotFound();
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);

            if (payment == null) return NotFound();
            Console.WriteLine($"Total amount: {booking.TotalAmount}");
            decimal fakeAmount = 10000;
            string bank = "Mbbank";
            string qrLink = $"https://qr.sepay.vn/img?acc={_paymentService._accountNumber}&bank={bank}&amount={fakeAmount}&des=Booking{booking.BookingId}";
            var viewModel = new CheckoutViewModel
            {
                Booking = booking,
                Payment = payment,
                Url = qrLink
            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
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
                foreach (var seatId in request.SeatIds)
                {
                    Console.WriteLine("seat id: " + seatId);
                }
                Console.WriteLine("Booking id: " + booking.BookingId);
                Console.WriteLine("user id: " + userId);
                bool ticketResult = await _bookingService.InsertTickets(request.SeatIds, booking.BookingId, userId, extraPrice, basePrice);
                if (!ticketResult)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Không thể thêm vé");
                }

                // 3. Lưu thay đổi và commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { BookingId = booking.BookingId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Tạo booking thất bại");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPayment(string paymentCode, decimal expectedAmount)
        {
            PaymentResult result = await _paymentService.VerifyPaymentAsync(paymentCode, expectedAmount);
            return Ok(
                new
                {
                    status = result.Success
                }
            );
        }
    }
}
