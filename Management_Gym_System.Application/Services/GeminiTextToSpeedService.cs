using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace Management_Gym_System.Application.Services.TextToSpeed
{
    public class GeminiTextToSpeedService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _voiceName;

        public GeminiTextToSpeedService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiTextToSpeed:ApiTextToSpeed"]
                      ?? throw new ArgumentNullException("Thiếu cấu hình GeminiTextToSpeed:ApiTextToSpeed");

            // Mặc định dùng "vi-VN-Neural2-A" (Giọng Nữ tự nhiên thế hệ mới - miễn phí 1 triệu ký tự/tháng)
            // Nếu muốn dùng gói 4 triệu ký tự/tháng thì đổi thành "vi-VN-Standard-A"
            _voiceName = configuration["GeminiTextToSpeed:VoiceName"] ?? "vi-VN-Neural2-A";
        }

        public async Task<string> ConvertTextToSpeechBase64Async(string textToRead)
        {
            string requestUrl = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={_apiKey}";

            var requestPayload = new
            {
                input = new
                {
                    text = textToRead
                },
                voice = new
                {
                    languageCode = "vi-VN",
                    name = _voiceName
                },
                audioConfig = new
                {
                    audioEncoding = "LINEAR16",
                    speakingRate = 1.0,
                    pitch = 0.0
                }
            };

            string jsonPayload = JsonSerializer.Serialize(requestPayload);
            using var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using HttpResponseMessage httpResponse = await _httpClient.PostAsync(requestUrl, requestContent);
            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Lỗi gọi Google Cloud Text-To-Speech: {responseBody}");
            }

            JsonNode? rootNode = JsonNode.Parse(responseBody);
            string? audioBase64 = rootNode?["audioContent"]?.GetValue<string>();

            if (string.IsNullOrEmpty(audioBase64))
            {
                throw new Exception("Google Cloud Text-To-Speech không trả về dữ liệu âm thanh.");
            }

            return audioBase64;
        }
    }
}