using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;

namespace DoAn.Areas.Booking.Services
{
    public class PaymentService
    {
        public readonly string _accountNumber;
        private readonly IHubContext<PaymentHub> _hubContext;
        public PaymentService(HttpClient httpClient, IConfiguration configuration, IHubContext<PaymentHub> hubContext)
        {
            _accountNumber = configuration["Sepay:AccountNumber"] ?? "";
            _hubContext = hubContext;
        }

        public async Task NotifyPaymentResult(int userId, int bookingId, bool success)
        {
            try
            {
                Console.WriteLine($"Gửi thông báo đến User_{userId} - BookingId: {bookingId}, Success: {success}");
                await _hubContext.Clients
                    .Group($"User_{userId}")
                    .SendAsync("PaymentConfirmed", new
                    {
                        bookingId = bookingId,
                        success = success,
                        message = success ? "Thanh toán thành công" : "Thanh toán thất bại",
                        timestamp = DateTime.Now
                    });

                Console.WriteLine($"✓ Đã gửi thông báo thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Lỗi khi gửi thông báo: {ex.Message}");
            }
        }
    }
    
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ?Message { get; set; }
    }

    public class SepayPayload
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; }

        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("transferType")]
        public string TransferType { get; set; }

        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; }

        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        [JsonPropertyName("subAccount")]
        public string SubAccount { get; set; }

        [JsonPropertyName("referenceCode")]
        public string ReferenceCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class PaymentHub : Hub
    {
        private static readonly Dictionary<string, string> UserConnections =
            new Dictionary<string, string>();

        public async Task RegisterUser(string userId)
        {
            try
            {
                Console.WriteLine($"RegisterUser được gọi - UserId: {userId}, ConnectionId: {Context.ConnectionId}");

                // Lưu mapping
                UserConnections[userId] = Context.ConnectionId;

                // Thêm vào group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                Console.WriteLine($"✓ User {userId} đã được thêm vào group User_{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Lỗi RegisterUser: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
                if (!string.IsNullOrEmpty(userId))
                {
                    UserConnections.Remove(userId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                    Console.WriteLine($"User {userId} đã disconnect");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi OnDisconnectedAsync: {ex.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
