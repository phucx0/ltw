using Azure.Core;
using DoAn.Areas.Booking.Services;
using DoAn.Models.Accounts;
using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Data;
using DoAn.Models.Payments;
using DoAn.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAn.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class ReservationController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        private readonly BookingService _bookingService;
        private readonly PaymentService _paymentService;
        public ReservationController(BookingService bookingService, PaymentService paymentService, IDbContextFactory dbFactory)
        {
            _bookingService = bookingService;
            _paymentService = paymentService;
            _dbFactory = dbFactory;
        }

        public async Task<IActionResult> TestCreateBooking(int showtimeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new { success = false, message = "Not logged in" });
            // ds ghế đang giữ
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var heldSeats = db.SeatHold
                .Where(sh => sh.ShowtimeId == showtimeId && sh.UserId == int.Parse(userId))
                .ToList();

            if (heldSeats.Count() == 0) return BadRequest(new { success = false, message = "No seats held" });

            // giá của phòng
            decimal basePrice = await db.RoomTypes
                    .Where(rt => rt.Rooms.Any(r => r.Showtimes.Any(s => s.ShowtimeId == showtimeId)))
                    .Select(rt => rt.BasePrice)
                    .FirstOrDefaultAsync();
            // giá phụ thu theo loại ghế
            decimal extraPrice = await db.SeatTypes
                .Where(st => st.Seats.Any(s => s.SeatId == heldSeats[0].SeatId))
                .Select(st => st.ExtraPrice)
                .FirstOrDefaultAsync();
            Console.WriteLine("Base price: " + basePrice);
            Console.WriteLine("extra price: " + extraPrice);
            Console.WriteLine("Held seat: " + heldSeats.Count());
            decimal totalAmount = (basePrice + extraPrice) * heldSeats.Count();
            try
            {
                var bookingIdParam = new SqlParameter
                {
                    ParameterName = "@BookingId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };


                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_FinalizeBooking @UserId={0}, @ShowtimeId={1}, @Amount={2}, @BookingId=@BookingId OUTPUT",
                    userId, showtimeId, totalAmount, bookingIdParam
                );
                int bookingId = (int)bookingIdParam.Value;
                return Ok(new
                {
                    success = true,
                    message = "Booking created successfully",
                    bookingId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Booking failed",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        public async Task<IActionResult> Checkout(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var booking = await db.Bookings
                .Where(b => b.BookingId == bookingId && b.UserId == userId && b.Status == "pending")
                .Select(b => new { b.BookingId, b.Status })
                .FirstOrDefaultAsync();
            if (booking == null)
                return RedirectToAction("Movies", "Movie", new { area = "" });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(booking.BookingId);
            Console.ResetColor();

            var fullBooking = await db.Bookings
                .Include(b => b.Showtime).ThenInclude(s => s.Movie)
                .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Branch)
                //.Include(b => b.Showtime).ThenInclude(s => s.).ThenInclude(r => r.Branch)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);


            if (fullBooking == null)
                return NotFound();
            var payment = await db.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
            if (payment == null) return NotFound();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(payment.PaymentId);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Total amount: {fullBooking.TotalAmount}");
            Console.ResetColor();

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
        public async Task<IActionResult> CreateBooking(int showtimeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new { success = false, message = "Not logged in" });
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            // ds ghế đang giữ
            var heldSeats = db.SeatHold
                .Where(sh => sh.ShowtimeId == showtimeId && sh.UserId == int.Parse(userId))
                .ToList();

            if (heldSeats.Count() == 0) return BadRequest(new { success = false, message = "No seats held" });

            // giá của phòng
            decimal basePrice = await db.RoomTypes
                    .Where(rt => rt.Rooms.Any(r => r.Showtimes.Any(s => s.ShowtimeId == showtimeId)))
                    .Select(rt => rt.BasePrice)
                    .FirstOrDefaultAsync();
            // giá phụ thu theo loại ghế
            decimal extraPrice = await db.SeatTypes
                .Where(st => st.Seats.Any(s => s.SeatId == heldSeats[0].SeatId))
                .Select(st => st.ExtraPrice)
                .FirstOrDefaultAsync();

            decimal totalAmount = (basePrice + extraPrice) * heldSeats.Count();
            try
            {
                var bookingIdParam = new SqlParameter
                {
                    ParameterName = "@BookingId",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };


                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_FinalizeBooking @UserId={0}, @ShowtimeId={1}, @Amount={2}, @BookingId=@BookingId OUTPUT",
                    userId, showtimeId, totalAmount, bookingIdParam
                );
                int bookingId = (int)bookingIdParam.Value;
                return Ok(new
                {
                    success = true,
                    message = "Booking created successfully",
                    bookingId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Booking failed",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            var booking = await db.Bookings
                .Include(b => b.BookingSeats)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);
            if (booking != null)
            {
                Console.WriteLine(booking.BookingId);
                // Xóa booking_seats
                db.BookingSeat.RemoveRange(booking.BookingSeats);
                // Xóa Payments
                db.Payments.RemoveRange(booking.Payments);
                // Xóa booking
                db.Bookings.Remove(booking);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Movies", "Movie", new { area = "" });
        }
    }
}
