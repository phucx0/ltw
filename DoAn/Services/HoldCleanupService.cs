using DoAn.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Services
{
    public class HoldCleanupService : BackgroundService
    {
        private readonly IDbContextFactory _dbFactory;
        private readonly ILogger<HoldCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _errorRetryInterval = TimeSpan.FromMinutes(5);

        public HoldCleanupService(
            IDbContextFactory dbFactory,
            ILogger<HoldCleanupService> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HoldCleanupService started");

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredHolds();
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // App đang shutdown, thoát gracefully
                    _logger.LogInformation("HoldCleanupService is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "Error in HoldCleanupService. Retrying in {RetryInterval} minutes",
                    //    _errorRetryInterval.TotalMinutes);
                    try
                    {
                        await Task.Delay(_errorRetryInterval, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            _logger.LogInformation("HoldCleanupService stopped");
        }

        private async Task CleanupExpiredHolds()
        {
            try
            {
                using var db = _dbFactory.Create("MOVIE_TICKET_db", "app_user", "app123");

                var now = DateTime.Now;
                var expiredHolds = await db.SeatHold
                    .Where(sh => sh.ExpireAt <= now)
                    .ToListAsync();

                if (expiredHolds.Any())
                {
                    db.SeatHold.RemoveRange(expiredHolds);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to cleanup expired holds");
                throw; // Ném lên để ExecuteAsync bắt và retry
            }
        }
    }
}