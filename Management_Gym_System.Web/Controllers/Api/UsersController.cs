using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IGenericService<User> _userService;
        private readonly ApplicationDbContext _context;

        public UsersController(IGenericService<User> userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Users/Index.cshtml");
        }

        [HttpGet("listUsers")]
        public async Task<IActionResult> GetUsers(string? keyword, long? filterValue)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Memberships)
                    .ThenInclude(m => m.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = StringHelper.NormalizeText(keyword);
                query = query.Where(u =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(u.FullName),
                        $"%{normalizedKeyword}%"));
            }

            if (filterValue.HasValue)
            {
                query = query.Where(u =>
                    u.Memberships.Any(m => m.ProductID == filterValue.Value));
            }

            var users = await query.OrderBy(u => u.ID).ToListAsync();

            var result = users.Select(u => new
            {
                id = u.ID,
                fullName = u.FullName,
                phoneNumber = u.PhoneNumber,
                roleName = u.Role?.RoleName,
                avatar = u.Avatar,
                status = u.Status,
                roleId = u.RoleID,
                goiTapId = u.Memberships.Select(m => m.ProductID).FirstOrDefault(),
                goiTapName = u.Memberships.Select(m => m.Product?.ProductName).FirstOrDefault()
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo user
            var user = new User
            {
                RoleID = request.RoleID,
                FullName = request.FullName,
                Avatar = request.Avatar,
                PhoneNumber = request.PhoneNumber,
                Status = request.Status
            };

            // Lưu user trước để có ID
            await _userService.AddAsync(user);

            // Nếu có gói tập thì tạo thẻ tập
            if (request.GoiTapID.HasValue)
            {
                // Tìm thẻ chưa gán user nhưng đã có RFID
                var membership = await _context.GymMembershipCards
                    .FirstOrDefaultAsync(x =>
                        x.UserID == null &&
                        !string.IsNullOrEmpty(x.RFID_UID) &&
                        x.Status == true);

                // Không còn thẻ trống
                if (membership == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không còn thẻ khả dụng"
                    });
                }

                var startDate = DateTime.UtcNow;

                // Map user vào thẻ
                membership.UserID = user.ID;
                membership.ProductID = request.GoiTapID;

                membership.StartDate = startDate;

                membership.EndDate = request.ThoiHan.HasValue
                    ? startDate.AddDays(request.ThoiHan.Value)
                    : null;

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                success = true,
                data = user
            });
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserCreateUpdateDto request)
        {
            if (id != request.Id)
                return BadRequest();

            var existing = await _userService.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            // Update user
            existing.FullName = request.FullName;
            existing.PhoneNumber = request.PhoneNumber;
            existing.RoleID = request.RoleID;
            existing.Avatar = request.Avatar;
            existing.Status = request.Status;

            await _userService.UpdateAsync(existing);

            // Tìm thẻ tập hiện tại của user
            var membership = await _context.GymMembershipCards
                .FirstOrDefaultAsync(x => x.UserID == id);

            // Nếu chưa có thẻ thì tạo mới
            if (membership == null)
            {
                if (request.GoiTapID.HasValue)
                {
                    var startDate = DateTime.UtcNow;

                    membership = new GymMembershipCard
                    {
                        UserID = id,
                        ProductID = request.GoiTapID,
                        StartDate = startDate,
                        EndDate = request.ThoiHan.HasValue
                            ? startDate.AddDays(request.ThoiHan.Value)
                            : null,

                        Status = true
                    };

                    _context.GymMembershipCards.Add(membership);
                }
            }
            else
            {
                // Update gói tập
                membership.ProductID = request.GoiTapID;

                // Reset thời gian nếu có thời hạn mới
                if (request.ThoiHan.HasValue)
                {
                    membership.StartDate = DateTime.UtcNow;
                    membership.EndDate = DateTime.UtcNow.AddDays(request.ThoiHan.Value);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Status = !existing.Status;
            await _userService.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });

            // Reset thẻ của user về null để người khác dùng
            var card = await _context.GymMembershipCards.FirstOrDefaultAsync(c => c.UserID == id);
            if (card != null)
            {
                card.UserID = null;
                card.ProductID = null;
                card.StartDate = null;
                card.EndDate = null;
                card.PauseDate = null;
                card.ResumeDate = null;
                card.Status = false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    public class UserCreateUpdateDto
    {
        public long? Id { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public long? RoleID { get; set; }

        public long? GoiTapID { get; set; }

        public int? ThoiHan { get; set; }

        public string? Avatar { get; set; }

        public bool? Status { get; set; }
    }

}