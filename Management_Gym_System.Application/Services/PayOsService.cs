using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace Management_Gym_System.Application.Services.PayOs
{
    public class PayOSQrResult
    {
        public long OrderCode { get; set; }
        public string QrCodeData { get; set; } = string.Empty;
    }

    public class PayOSStatusResult
    {
        public string Status { get; set; } = string.Empty; // PENDING, PAID, CANCELLED, EXPIRED
        public int Amount { get; set; }
    }

    public class PayOSService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;
        private readonly string _partnerCode;

        public PayOSService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("Thiếu cấu hình PayOS:ClientId");
            _apiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("Thiếu cấu hình PayOS:ApiKey");
            _checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("Thiếu cấu hình PayOS:ChecksumKey");
            _partnerCode = configuration["PayOS:PartnerCode"] ?? string.Empty;
        }

        // Nhận đầu vào là số tiền, trả về mã đơn hàng và chuỗi dữ liệu để vẽ QR
        public async Task<PayOSQrResult> CreateQrPaymentAsync(int paymentAmount, string returnUrl, string cancelUrl)
        {
            // Dùng UnixTimeSeconds (10 chữ số) để nội dung chuyển khoản không vượt quá giới hạn 25 ký tự của PayOS
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string paymentDescription = $"Thanh toan {orderCode}";

            // PayOS bắt buộc dữ liệu ký HMAC-SHA256 phải xếp theo thứ tự bảng chữ cái: amount -> cancelUrl -> description -> orderCode -> returnUrl
            string rawSignatureData = $"amount={paymentAmount}&cancelUrl={cancelUrl}&description={paymentDescription}&orderCode={orderCode}&returnUrl={returnUrl}";
            string signature = ComputeHmacSha256(rawSignatureData, _checksumKey);

            var requestPayload = new
            {
                orderCode = orderCode,
                amount = paymentAmount,
                description = paymentDescription,
                cancelUrl = cancelUrl,
                returnUrl = returnUrl,
                signature = signature
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v2/payment-requests");
            httpRequest.Headers.Add("x-client-id", _clientId);
            httpRequest.Headers.Add("x-api-key", _apiKey);

            if (!string.IsNullOrWhiteSpace(_partnerCode))
            {
                httpRequest.Headers.Add("x-partner-code", _partnerCode);
            }

            string jsonPayload = JsonSerializer.Serialize(requestPayload);
            httpRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            JsonNode? rootNode = JsonNode.Parse(responseBody);
            string? responseCode = rootNode?["code"]?.GetValue<string>();

            if (responseCode != "00")
            {
                string errorDescription = rootNode?["desc"]?.GetValue<string>() ?? "Lỗi không xác định từ PayOS";
                throw new Exception($"Không thể tạo mã QR PayOS: {errorDescription}");
            }

            // Lấy chuỗi dữ liệu VietQR (trường qrCode) để gửi về cho JavaScript vẽ hình mã QR
            string qrCodeData = rootNode?["data"]?["qrCode"]?.GetValue<string>() ?? string.Empty;

            return new PayOSQrResult
            {
                OrderCode = orderCode,
                QrCodeData = qrCodeData
            };
        }

        // Kiểm tra xem người dùng đã quét mã và chuyển khoản thành công hay chưa
        public async Task<PayOSStatusResult> GetPaymentStatusAsync(long orderCode)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api-merchant.payos.vn/v2/payment-requests/{orderCode}");
            httpRequest.Headers.Add("x-client-id", _clientId);
            httpRequest.Headers.Add("x-api-key", _apiKey);

            using HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            JsonNode? rootNode = JsonNode.Parse(responseBody);
            string status = rootNode?["data"]?["status"]?.GetValue<string>() ?? "PENDING";
            int amount = rootNode?["data"]?["amount"]?.GetValue<int>() ?? 0;

            return new PayOSStatusResult
            {
                Status = status.ToUpperInvariant(),
                Amount = amount
            };
        }

        private string ComputeHmacSha256(string rawData, string secretKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] dataBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = HMACSHA256.HashData(keyBytes, dataBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
