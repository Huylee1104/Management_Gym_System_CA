using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/checkin")]
    [ApiController]
    public class CheckinController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckinController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Checkins/Index.cshtml");
        }

        [HttpGet("listCheckins")]
        public async Task<IActionResult> GetCheckins([FromQuery] DateTime? date)
        {
            var filterDate = date?.Date ?? DateTime.Today;

            var checkins = await _context.Checkins
                .Include(c => c.Card)
                    .ThenInclude(card => card.User)
                .Where(c => c.CheckinTime.HasValue && c.CheckinTime.Value >= filterDate && c.CheckinTime.Value < filterDate.AddDays(1))
                .OrderByDescending(c => c.CheckinTime)
                .ToListAsync();

            var result = checkins.Select(c => new
            {
                checkinId = c.ID,
                checkinTime = c.CheckinTime.Value.ToString("HH:mm:ss"),
                cardID = c.CardID,
                fullName = c.Card.User.FullName,
                avatar = c.Card.User.Avatar ?? "https://via.placeholder.com/50",
                endDate = c.Card.EndDate.HasValue ? c.Card.EndDate.Value.ToString("dd/MM/yyyy") : null,
                rfidUid = c.Card.RFID_UID,
                cardStatus = c.Status ?? "active"
            });

            return Ok(result);
        }


        // 3. Thực hiện Quét thẻ (Check-in)
        [HttpPost("{RFID_UID}")]
        public async Task<IActionResult> DoCheckin(string RFID_UID)
        {
            var card = await _context.GymMembershipCards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.RFID_UID == RFID_UID);

            if (card == null) return NotFound("Không tìm thấy thẻ!");

            var cardStatus = card.Status == false ? "locked"
            : card.EndDate.HasValue && card.EndDate.Value < DateTime.Now ? "expired"
            : "active";

            var checkin = new Checkin
            {
                CardID = card.ID,
                CheckinTime = DateTime.Now,
                Status = cardStatus
            };

            _context.Checkins.Add(checkin);
            await _context.SaveChangesAsync();


            var startDate = card.StartDate.HasValue ? card.StartDate.Value.ToString("dd/MM/yyyy") : null;
            var endDate = card.EndDate.HasValue ? card.EndDate.Value.ToString("dd/MM/yyyy") : null;

            var result = new
            {
                Message = "Check-in thành công!",
                CardInfo = new
                {
                    card.ID,
                    RfidUid = card.RFID_UID,
                    card.User.FullName,
                    card.User.PhoneNumber,
                    Avatar = card.User.Avatar ?? "https://placehold.co/150",
                    startDate,
                    endDate,
                    cardStatus
                }
            };

            return Ok(result);
        }

        // 4. Gia hạn
        [HttpPost("extend/{cardId}")]
        public async Task<IActionResult> ExtendCard(long cardId)
        {
            var card = await _context.GymMembershipCards
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.ID == cardId);

            if (card == null) return NotFound();

            if (card.Product?.ThoiHan == null)
                return BadRequest(new { Message = "Gói tập của thẻ này chưa có thời hạn, không thể gia hạn!" });

            int thoiHan = card.Product.ThoiHan.Value;
            var now = DateTime.Now;

            var baseDate = card.EndDate.HasValue && card.EndDate.Value > now
                ? card.EndDate.Value
                : now;

            card.EndDate = baseDate.AddDays(thoiHan);
            card.Status = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Đã gia hạn thêm {thoiHan} ngày!",
                NewEndDate = card.EndDate?.ToString("dd/MM/yyyy")
            });
        }

        // 5. Khóa thẻ (Xóa mềm User & Card)
        [HttpPost("lock/{cardId}")]
        public async Task<IActionResult> LockCard(long cardId)
        {
            var card = await _context.GymMembershipCards
                .FirstOrDefaultAsync(c => c.ID == cardId);

            if (card == null) return NotFound();

            card.Status = false;
            card.PauseDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đã khóa thẻ hội viên!" });
        }

        // 6. Mở lại thẻ (Nếu đã khóa)
        [HttpPost("unlock/{cardId}")]
        public async Task<IActionResult> UnlockCard(long cardId)
        {
            var card = await _context.GymMembershipCards
                .FirstOrDefaultAsync(c => c.ID == cardId);

            if (card == null) return NotFound();

            if (card.PauseDate == null)
                return BadRequest(new { Message = "Thẻ này chưa có ngày tạm dừng, không thể mở lại!" });

            var resumeDate = DateTime.Now;
            int soNgayTamDung = (int)(resumeDate - card.PauseDate.Value).TotalDays;

            card.ResumeDate = resumeDate;
            card.EndDate = card.EndDate.HasValue
                ? card.EndDate.Value.AddDays(soNgayTamDung)
                : null;
            card.Status = true;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = $"Đã mở lại thẻ! Gia hạn thêm {soNgayTamDung} ngày do tạm dừng.",
                NewEndDate = card.EndDate?.ToString("dd/MM/yyyy")
            });
        }

        [HttpGet("latestToday")]
        public async Task<IActionResult> GetLatestToday()
        {
            var today = DateTime.Today;

            var latest = await _context.Checkins
                .Include(c => c.Card)
                    .ThenInclude(c => c.User)
                .Where(c => c.CheckinTime >= today && c.CheckinTime < today.AddDays(1))
                .OrderByDescending(c => c.CheckinTime)
                .FirstOrDefaultAsync();

            if (latest == null) return Ok(null);

            var card = latest.Card;
            var user = card.User;

            var cardStatus = card.Status == false ? "locked"
                        : card.EndDate.HasValue && card.EndDate.Value < DateTime.Now ? "expired"
                        : "active";

            var startDate = card.StartDate.HasValue ? card.StartDate.Value.ToString("dd/MM/yyyy") : null;
            var endDate = card.EndDate.HasValue ? card.EndDate.Value.ToString("dd/MM/yyyy") : null;

            var result = new
            {
                id = latest.CardID,
                rfidUid = card.RFID_UID,
                fullName = user.FullName,
                phoneNumber = user.PhoneNumber,
                avatar = user.Avatar ?? "https://placehold.co/150",
                startDate,
                endDate,
                cardStatus
            };

            return Ok(result);
        }
    }
}