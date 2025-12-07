using DoAn.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace DoAn.Controllers
{
    public class SeatHoldController : Controller
    {
        private readonly IDbContextFactory _dbFactory;
        private readonly int _holdMinutes = 5;
        public SeatHoldController(IDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<bool> HoldSeatAsync(int seatId, int showtimeId, int userId)
        {
            var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
            var outputParam = new SqlParameter("@Result", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Output
            };
            await db.Database.ExecuteSqlRawAsync(
                "EXEC HoldSeat @SeatId, @ShowtimeId, @UserId, @ExpireMinutes, @Result OUTPUT",
                new SqlParameter("@SeatId", seatId),
                new SqlParameter("@ShowtimeId", showtimeId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ExpireMinutes", _holdMinutes),
                outputParam
            );

            return (bool)outputParam.Value;
        }
        [HttpPost]
        public async Task<IActionResult> Hold(int seatId, int showtimeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new { success = false, message = "Not logged in" });
            bool holdSuccess = false;
            try
            {
                holdSuccess = await HoldSeatAsync(seatId, showtimeId, int.Parse(userId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }

            if (!holdSuccess)
                return Ok(new { success = false, message = "Seat already held or booked" });

            return Ok(new { success = true, message = "Seat held successfully" });
        }

        [HttpPost]
        public IActionResult Release(int seatId, int showtimeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new { success = false, message = "Not logged in" });

            using (var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123"))
            {
                var hold = db.SeatHold.FirstOrDefault(x =>
                    x.SeatId == seatId &&
                    x.ShowtimeId == showtimeId &&
                    x.UserId == int.Parse(userId)
                );

                if (hold != null)
                {
                    db.SeatHold.Remove(hold);
                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
        }
    }
}
