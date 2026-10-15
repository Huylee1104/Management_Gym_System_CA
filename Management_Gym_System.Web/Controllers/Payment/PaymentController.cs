using System.Diagnostics;
using Management_Gym_System.Application.Services.PayOs;
using Management_Gym_System.Application.Services.TextToSpeed;
using Management_Gym_System.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Controllers
{
    public class CreateQrRequest
    {
        public int PaymentAmount { get; set; }
    }

    public class PaymentIntegrationResponse
    {
        public string Status { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public string? AudioBase64 { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSService _payOSService;
        private readonly GeminiTextToSpeedService _geminiTextToSpeedService;

        public PaymentController(PayOSService payOSService, GeminiTextToSpeedService geminiTextToSpeedService)
        {
            _payOSService = payOSService;
            _geminiTextToSpeedService = geminiTextToSpeedService;
        }

        // Bước 1: Nhận số tiền từ JS, gọi PayOS lấy dữ liệu vẽ mã QR
        [HttpPost("create-qr")]
        public async Task<IActionResult> CreateQr([FromBody] CreateQrRequest request)
        {
            string currentDomain = $"{Request.Scheme}://{Request.Host}";
            PayOSQrResult qrResult = await _payOSService.CreateQrPaymentAsync(
                request.PaymentAmount,
                returnUrl: $"{currentDomain}/Payment/Success",
                cancelUrl: $"{currentDomain}/Payment/Cancel"
            );

            return Ok(new
            {
                orderCode = qrResult.OrderCode,
                qrCodeData = qrResult.QrCodeData
            });
        }

        // Bước 2: Endpoint để JS hỏi trạng thái thanh toán và nhận chuỗi âm thanh Base64
        [HttpGet("check-and-speak/{orderCode}")]
        public async Task<IActionResult> CheckPaymentStatusAndSpeak(long orderCode)
        {
            PaymentIntegrationResponse result = await ProcessPaymentAndGenerateSpeechAsync(orderCode);
            return Ok(result);
        }

        // HÀM TÍCH HỢP 2 SERVICE: Kiểm tra PayOS -> Nếu Thành công hoặc Thất bại thì gọi Gemini đọc thành Base64
        private async Task<PaymentIntegrationResponse> ProcessPaymentAndGenerateSpeechAsync(long orderCode)
        {
            PayOSStatusResult paymentStatus = await _payOSService.GetPaymentStatusAsync(orderCode);

            // Nếu khách vẫn đang quét mã (chưa chuyển khoản xong), báo về cho JS tiếp tục đợi, chưa gọi Gemini
            if (paymentStatus.Status == "PENDING" || paymentStatus.Status == "PROCESSING")
            {
                return new PaymentIntegrationResponse
                {
                    Status = "PENDING",
                    MessageText = "Đang chờ khách hàng quét mã thanh toán...",
                    AudioBase64 = null
                };
            }

            // Khi giao dịch đã có kết quả (Thành công hoặc Thất bại/Hủy), tạo nội dung để Gemini đọc
            string textForGeminiToRead;

            if (paymentStatus.Status == "PAID")
            {
                textForGeminiToRead = $"Thanh toán thành công số tiền {paymentStatus.Amount:N0} đồng. Xin cảm ơn quý khách.";
            }
            else
            {
                textForGeminiToRead = "Giao dịch thanh toán thất bại hoặc đã bị hủy. Vui lòng kiểm tra lại.";
            }

            // Gọi Gemini chuyển văn bản thành chuỗi Base64
            string audioBase64 = await _geminiTextToSpeedService.ConvertTextToSpeechBase64Async(textForGeminiToRead);

            return new PaymentIntegrationResponse
            {
                Status = paymentStatus.Status,
                MessageText = textForGeminiToRead,
                AudioBase64 = audioBase64
            };
        }
    }
}