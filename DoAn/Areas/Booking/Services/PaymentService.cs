using Azure.Core;
using DoAn.Models.Booking;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoAn.Areas.Booking.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string url = "https://my.sepay.vn";
        private readonly string _apiKey;
        public readonly string _accountNumber;
        public PaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _accountNumber = configuration["Sepay:AccountNumber"] ?? "";
            _apiKey = configuration["Sepay:ApiKey"] ?? "";
        }

        public async Task<PaymentResult> VerifyPaymentAsync(string transactionContent, decimal expectedAmount)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return new PaymentResult { Success = false, Message = "API KEY không tồn tại" };
            }
            string api_url = $"{url}/userapi/transactions/list?account_number={_accountNumber}&limit=20";
            Console.WriteLine("Url: " + api_url);
            Console.WriteLine("API KEY: " + _apiKey);
            Console.WriteLine("Transaction content: " + transactionContent);
            // 1. Gọi API Sepay lấy thông tin giao dịch
            var request = new HttpRequestMessage(HttpMethod.Get, api_url);

            // Thêm header
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Nếu API yêu cầu Content-Type (một số GET không cần, nhưng để chắc)
            request.Content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Không thể kết nối Sepay");
                return new PaymentResult { Success = false, Message = "Không thể kết nối Sepay" };
            }

            var data = await response.Content.ReadFromJsonAsync<SepayResponse>();

            if (data == null || data.Transactions == null)
            {
                Console.WriteLine("Không nhận được dữ liệu giao dịch");
                return new PaymentResult { Success = false, Message = "Không nhận được dữ liệu giao dịch" };
            }

            // Tìm giao dịch khớp với PaymentCode
            foreach (var txh in data.Transactions)
            {
                Console.WriteLine("Amount in: " + txh.AmountIn);
                Console.WriteLine("transaction_content: " + txh.TransactionContent);
            }
            var transaction = data.Transactions.FirstOrDefault(t =>
                t.AmountIn == expectedAmount &&
                t.TransactionContent.Contains(transactionContent, StringComparison.OrdinalIgnoreCase)
            );

            if (transaction == null)
            {
                Console.WriteLine("Chưa thanh toán hoặc số tiền không khớp");
                return new PaymentResult { Success = false, Message = "Chưa thanh toán hoặc số tiền không khớp" };
            }

            return new PaymentResult { Success = true, Transaction = transaction };
        }
    }
    public class SepayResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("messages")]
        public SepayMessages Messages { get; set; }

        [JsonPropertyName("transactions")]
        public List<SepayTransaction> Transactions { get; set; }
    }

    public class SepayMessages
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class SepayTransaction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("bank_brand_name")]
        public string BankBrandName { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("transaction_date")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("amount_out")]
        public decimal AmountOut { get; set; }

        [JsonPropertyName("amount_in")]
        public decimal AmountIn { get; set; }

        [JsonPropertyName("accumulated")]
        public decimal Accumulated { get; set; }

        [JsonPropertyName("transaction_content")]
        public string TransactionContent { get; set; }

        [JsonPropertyName("reference_number")]
        public string ReferenceNumber { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("sub_account")]
        public string SubAccount { get; set; }

        [JsonPropertyName("bank_account_id")]
        public string BankAccountId { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ?Message { get; set; }
        public SepayTransaction ?Transaction { get; set; }
    }
}
