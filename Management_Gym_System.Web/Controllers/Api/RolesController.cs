using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IGenericService<UserRole> _roleService;
        private readonly ApplicationDbContext _context;

        public RolesController(IGenericService<UserRole> roleService, ApplicationDbContext context)
        {
            _roleService = roleService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Roles/Index.cshtml");
        }

        // GET: /api/roles
        [HttpGet("listRoles")]
        public async Task<IActionResult> GetRoles(string? keyword)
        {
            var query = _context.UserRoles.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                var normalizedKeyword = StringHelper.NormalizeText(keyword);
                query = query.Where(r =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(r.RoleName),
                        $"%{normalizedKeyword}%"));
            }

            var roles = await query.OrderBy(r => r.ID).ToListAsync();
            return Ok(roles);
        }

        // POST: /api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] UserRole role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            await _roleService.AddAsync(role);
            return CreatedAtAction(nameof(GetRoles), new { id = role.ID }, role);
        }

        // POST: /api/roles/{id}
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UserRole role)
        {
            if (id != role.ID) return BadRequest("ID mismatch");

            var existingRole = await _roleService.GetByIdAsync(id);
            if (existingRole == null) return NotFound();

            existingRole.RoleName = role.RoleName;
            existingRole.Status = role.Status;
            
            await _roleService.UpdateAsync(existingRole);
            return Ok(existingRole);
        }

        // POST: /api/roles/{id}/status
        [HttpPost("{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var existingRole = await _roleService.GetByIdAsync(id);
            if (existingRole == null) return NotFound();

            existingRole.Status = !existingRole.Status; // Đảo trạng thái
            await _roleService.UpdateAsync(existingRole);
            return Ok(new { Message = "Status updated", NewStatus = existingRole.Status });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.UserRoles.FindAsync(id);
            if (role == null)
                return Json(new { success = false, message = "Không tìm thấy vai trò!" });

            _context.UserRoles.Remove(role);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}