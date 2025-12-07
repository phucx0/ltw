using DoAn.Helpers;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoAn.Areas.Booking.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        public readonly string _accountNumber;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly IHubContext<PaymentHub> _hubContext;
        public PaymentService(HttpClient httpClient, IConfiguration configuration, IHubContext<PaymentHub> hubContext)
        {
            _httpClient = httpClient;
            _accountNumber = configuration["Sepay:AccountNumber"] ?? "";
            _apiUrl = configuration["Sepay:Url"] ?? "";
            _apiKey = configuration["Sepay:ApiKey"] ?? "";
            _hubContext = hubContext;
        }
        /// <summary>
        /// Lấy chi tiết giao dịch từ Sepay theo transactionId
        /// </summary>
        /// 
        public async Task<SepayTransaction?> GetTransactionDetailsAsync(int transactionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/userapi/transactions/details/{transactionId}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                // log lỗi, throw hoặc return null
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Raw JSON Response:");
            Console.WriteLine(content);
            try
            {
                var result = JsonSerializer.Deserialize<SepayTransactionResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new SepayDateTimeConverter() }
                });
                if (result == null || result.Transaction == null) return null;
                return result.Transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization Error: {ex.Message}");
                Console.WriteLine($"Raw content: {content}");
                return null;
            }
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


    public class SepayTransactionResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("messages")]
        public SepayMessages Messages { get; set; } = null!;

        [JsonPropertyName("transaction")]
        public SepayTransaction? Transaction { get; set; }
    }

    public class SepayMessages
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class SepayTransaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; } = null!;

        [JsonPropertyName("sub_account")]
        public string SubAccount { get; set; } = null!;

        [JsonPropertyName("amount_in")]
        public string AmountIn { get; set; }

        [JsonPropertyName("amount_out")]
        public string AmountOut { get; set; }

        [JsonPropertyName("accumulated")]
        public string Accumulated { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("transaction_content")]
        public string TransactionContent { get; set; } = null!;

        [JsonPropertyName("reference_number")]
        public string ReferenceNumber { get; set; } = null!;

        [JsonPropertyName("bank_brand_name")]
        public string BankBrandName { get; set; } = null!;

        [JsonPropertyName("bank_account_id")]
        public string BankAccountId { get; set; } = null!;
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
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
