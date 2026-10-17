using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Management_Gym_System.Application.Services.Cloudinary
{
    public class CloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        // 1. Upload qua IFormFile
        public async Task<string> UploadImageAsync(IFormFile file, string folder = "products", string? customName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File ảnh không hợp lệ hoặc rỗng.");

            await using var stream = file.OpenReadStream();
            return await UploadImageAsync(stream, file.FileName, folder, customName);
        }

        // 2. Upload qua mảng byte[]
        public async Task<string> UploadImageAsync(byte[] bytes, string originalFileName, string folder = "products", string? customName = null)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentException("Dữ liệu byte ảnh không hợp lệ.");

            using var memoryStream = new MemoryStream(bytes);
            return await UploadImageAsync(memoryStream, originalFileName, folder, customName);
        }

        // 3. Hàm xử lý gốc qua Stream (Tối ưu hiệu năng nhất)
        public async Task<string> UploadImageAsync(Stream stream, string originalFileName, string folder = "products", string? customName = null)
        {
            // Tự sinh tên file chuẩn: ten-anh-yyyyMMddHHmmss-guid
            string publicId = GenerateUniquePublicId(customName ?? Path.GetFileNameWithoutExtension(originalFileName));

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(originalFileName, stream),
                PublicId = publicId,
                Folder = folder,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Lỗi upload Cloudinary: {uploadResult.Error.Message}");
            }

            // Trả về link ảnh HTTPS an toàn
            return uploadResult.SecureUrl?.ToString() ?? string.Empty;
        }

        // Xóa ảnh khỏi Cloudinary
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        // Hàm phụ trợ tạo tên file chuẩn, loại bỏ dấu tiếng Việt và ký tự đặc biệt
        private string GenerateUniquePublicId(string inputName)
        {
            string slug = Regex.Replace(inputName.ToLower().Trim(), @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            if (string.IsNullOrEmpty(slug)) slug = "image";

            string timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string shortGuid = Guid.NewGuid().ToString("N")[..8];

            return $"{slug}-{timeStamp}-{shortGuid}";
        }
    }
}