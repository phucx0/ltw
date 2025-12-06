using DoAn.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Services
{
    public class HoldCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // chạy mỗi phút

        public HoldCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredHolds();
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CleanupExpiredHolds()
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ModelContext>();

            var expiredHolds = await _context.SeatHold
                .Where(sh => sh.ExpireAt <= DateTime.Now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                _context.SeatHold.RemoveRange(expiredHolds);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Removed {expiredHolds.Count} expired seat holds at {DateTime.Now}");
            }
        }
    }
}
