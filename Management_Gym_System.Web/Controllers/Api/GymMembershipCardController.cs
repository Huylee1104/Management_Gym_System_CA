using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Management_Gym_System.Controllers
{
    public class GymMembershipCardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymMembershipCardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Giao diện chính
        public IActionResult Index()
        {
            return View("~/Views/GymMembershipCard/Index.cshtml");
        }

        // API: Lấy danh sách thẻ theo bộ lọc
        [HttpGet]
        public async Task<IActionResult> GetCards(string? filter, string? keyword)
        {
            var query = _context.GymMembershipCards.AsQueryable();

            // Áp dụng bộ lọc
            switch (filter)
            {
                case "unregistered":
                    query = query.Where(c => string.IsNullOrEmpty(c.RFID_UID));
                    break;
                case "active":
                    query = query.Where(c => c.Status == true);
                    break;
                case "inactive":
                    query = query.Where(c => c.Status == false && !string.IsNullOrEmpty(c.RFID_UID));
                    break;
                case "all":
                default:
                    break;
            }

            // Tìm kiếm theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = StringHelper.NormalizeText(keyword);

                query = query.Where(c =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(c.User.FullName),
                        $"%{normalizedKeyword}%") ||
                    EF.Functions.ILike(
                        c.RFID_UID,
                        $"%{normalizedKeyword}%"));
            }

            var cards = await query
                .OrderByDescending(c => c.ID)
                .Include(c => c.User)
                .Include(c => c.Product)
                .ToListAsync();

            var result = cards
                .OrderByDescending(c => c.Status == true)
                .ThenBy(c => c.EndDate.HasValue 
                ? Math.Abs((c.EndDate.Value - DateTime.Now).TotalDays) 
                : double.MaxValue)
                .Select(c => new
                {
                    c.ID,
                    c.RFID_UID,
                    c.Status,
                    StartDate = c.StartDate?.ToString("dd/MM/yyyy") ?? "",
                    EndDate = c.EndDate?.ToString("dd/MM/yyyy") ?? "",
                    PauseDate = c.PauseDate?.ToString("dd/MM/yyyy") ?? "",
                    ResumeDate = c.ResumeDate?.ToString("dd/MM/yyyy") ?? "",
                    UserName = c.User?.FullName ?? "Chưa đăng ký",
                    ProductName = c.Product?.ProductName ?? "Chưa đăng ký"
                });

            return Json(new { success = true, data = result });
        }

        // API: Thêm mới thẻ trống
        [HttpPost]
        public async Task<IActionResult> GenerateCards(int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng thẻ phải lớn hơn 0." });
            }

            var newCards = new List<GymMembershipCard>();
            for (int i = 0; i < quantity; i++)
            {
                newCards.Add(new GymMembershipCard
                {
                    // Các trường khác đều null theo yêu cầu
                    RFID_UID = null,
                    UserID = null,
                    ProductID = null,
                    StartDate = null,
                    EndDate = null,
                    PauseDate = null,
                    ResumeDate = null,
                    Status = false
                });
            }

            await _context.GymMembershipCards.AddRangeAsync(newCards);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã thêm thành công {quantity} thẻ trống!" });
        }

        // API: Đăng ký / Cập nhật RFID_UID
        [HttpPost]
        public async Task<IActionResult> UpdateRFID(long id, string rfidUid)
        {
            var card = await _context.GymMembershipCards.FindAsync(id);
            if (card == null) return Json(new { success = false, message = "Không tìm thấy thẻ." });

            // Kiểm tra xem mã RFID này đã được thẻ khác đăng ký chưa (nếu cần)
            var exists = await _context.GymMembershipCards.AnyAsync(c => c.RFID_UID == rfidUid && c.ID != id);
            if (exists) return Json(new { success = false, message = "Mã RFID này đã tồn tại trên hệ thống!" });

            card.RFID_UID = rfidUid;
            
            // Nếu lần đầu đăng ký, có thể tự động chuyển Status sang true nếu bạn muốn
            // card.Status = true; 

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật mã RFID thành công!" });
        }

        // API: Khóa / Mở thẻ
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var card = await _context.GymMembershipCards.FindAsync(id);
            if (card == null) return Json(new { success = false, message = "Không tìm thấy thẻ." });

            // Nếu Status đang true -> false. Nếu false hoặc null -> true.
            card.Status = card.Status == true ? false : true;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = card.Status == true ? "Đã mở khóa thẻ!" : "Đã khóa thẻ!" });
        }

        // API: Xóa thẻ
        [HttpPost]
        public async Task<IActionResult> DeleteCard(long id)
        {
            var card = await _context.GymMembershipCards.FindAsync(id);
            if (card == null) return Json(new { success = false, message = "Không tìm thấy thẻ." });

            _context.GymMembershipCards.Remove(card);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa thẻ thành công!" });
        }
    }
}