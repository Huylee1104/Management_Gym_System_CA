using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Management_Gym_System.Application.Services;

namespace Management_Gym_System.Controllers
{
    public class GymMembershipCardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGymMembershipCardService _cardService;

        public GymMembershipCardController(ApplicationDbContext context, IGymMembershipCardService cardService)
        {
            _context = context;
            _cardService = cardService;
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
            var result = await _cardService.GetFilteredCardsAsync(filter, keyword);

            return Json(new { success = true, data = result });
        }

        // API: Thêm mới thẻ trống
        [HttpPost]
        public async Task<IActionResult> GenerateCards(int quantity)
        {
            var result = await _cardService.CreateCardQualityAsync(quantity);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        // API: Đăng ký / Cập nhật RFID_UID
        [HttpPost]
        public async Task<IActionResult> UpdateRFID(long id, string rfidUid)
        {
            var result = await _cardService.UpdateCardAsync(id, rfidUid);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        // API: Khóa / Mở thẻ
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _cardService.LockUnlockCardAsync(id);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }

        // API: Xóa thẻ
        [HttpPost]
        public async Task<IActionResult> DeleteCard(long id)
        {
            var result = await _cardService.DeleteCardAsync(id);
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}