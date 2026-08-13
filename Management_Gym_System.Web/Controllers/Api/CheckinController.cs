using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class CheckinController : Controller
    {
        private readonly ICheckinService _checkinService;

        public CheckinController(ICheckinService checkinService)
        {
            _checkinService = checkinService;
        }

        [HttpGet]
        [HasPermission("CHECKIN_VIEW")]
        public async Task<IActionResult> Index()
        {

            return View("~/Views/Checkins/Index.cshtml");
        }

        [HttpGet("listCheckins")]
        [HasPermission("CHECKIN_VIEW")]
        public async Task<IActionResult> GetCheckins([FromQuery] DateTime? date)
        {
            var result = await _checkinService.GetCheckinsAsync(date);

            return Ok(result);
        }


        // 3. Thực hiện Quét thẻ (Check-in)
        [HttpPost("{RFID_UID}")]
        [HasPermission("CHECKIN_EDIT")]
        public async Task<IActionResult> DoCheckin(string RFID_UID)
        {
            var result = await _checkinService.DoCheckin(RFID_UID);

            if (result == null || result.ID == 0)
            {
                return BadRequest(new { Message = "Thẻ hội viên không tồn tại!" });
            }

            return Ok(result);
        }

        // 4. Gia hạn
        [HttpPost("extend/{cardId}")]
        [HasPermission("CHECKIN_EDIT")]
        public async Task<IActionResult> ExtendCard(long cardId)
        {
            var newEndDate = await _checkinService.ExtendCard(cardId);

            if (newEndDate == null)
            {
                return BadRequest(new { Message = "Thẻ đã hết hạn hoặc không có ngày hết hạn!" });
            }

            return Ok(new
            {
                Message = $"Đã gia hạn thành công!",
                NewEndDate = newEndDate?.ToString("dd/MM/yyyy")
            });
        }

        // 5. Khóa thẻ (Xóa mềm User & Card)
        [HttpPost("lock/{cardId}")]
        [HasPermission("CHECKIN_DELETE")]
        public async Task<IActionResult> LockCard(long cardId)
        {
            var result = await _checkinService.LockCard(cardId);
            if (!result)
            {
                return BadRequest(new { Message = "Khóa thẻ thất bại!" });
            }
            return Ok(new { Message = "Đã khóa thẻ hội viên!" });
        }

        // 6. Mở lại thẻ (Nếu đã khóa)
        [HttpPost("unlock/{cardId}")]
        [HasPermission("CHECKIN_EDIT")]
        public async Task<IActionResult> UnlockCard(long cardId)
        {
            var newEndDate = await _checkinService.UnlockCard(cardId);
            if (newEndDate == null)
            {
                return BadRequest(new { Message = "Mở lại thẻ thất bại!" });
            }
            return Ok(new
            {
                Message = $"Đã mở lại thẻ hội viên!",
                NewEndDate = newEndDate?.ToString("dd/MM/yyyy")
            });
        }

        [HttpGet("latestToday")]
        public async Task<IActionResult> GetLatestToday()
        {
            var result = await _checkinService.GetLatestToday();

            return Ok(result);
        }
    }
}