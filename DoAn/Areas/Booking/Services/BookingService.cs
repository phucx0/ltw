using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Data;
using DoAn.Models.Payments;

namespace DoAn.Areas.Booking.Services
{
    public class BookingService
    {
        private readonly IDbContextFactory _dbFactory;

        public BookingService (IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;

        }
        public async Task<(Models.Booking.Booking booking, Payment payment)> CreateBooking(int userId, int showtimeId, decimal totalAmount)
        {
            string status = "pending";
            var booking = new Models.Booking.Booking
            {
                UserId = userId,
                ShowtimeId = showtimeId,
                Status = status,
                TotalAmount = totalAmount
            };
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");

            await db.Bookings.AddAsync(booking);
            await db.SaveChangesAsync();

            // Tạo payment record
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = totalAmount,
                Method = "Sepay_QR",
                Status = status,
                TransactionContent = $"Booking{booking.BookingId}",
                PaymentTime = DateTime.Now
            };
            await db.Payments.AddAsync(payment);
            //await _context.SaveChangesAsync();

            return (booking, payment);
        }

        public async Task<bool> InsertTickets(int bookingId, int userId)
        {
            try
            {
                var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
                var bookingSeats = db.BookingSeat
                    .Where(b => b.BookingId == bookingId)
                    .ToList();

                var seatIds = bookingSeats.Select(b => b.SeatId).ToList();
                var price = bookingSeats[0].Price;
                //decimal basePrice = _context.
                List<Ticket> tickets = new List<Ticket>();
                foreach (int seatId in seatIds)
                {
                    tickets.Add(new Ticket
                    {
                        SeatId = seatId,
                        UserId = userId,
                        Status = "booked",
                        BookingId = bookingId,
                        Price = price,
                        BookingTime = DateTime.Now
                    });
                }
                await db.Tickets.AddRangeAsync(tickets);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertTickets error: {ex}");
                return false;
            }
        }
    }
    public class BookingRequest
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; }
    }

}
