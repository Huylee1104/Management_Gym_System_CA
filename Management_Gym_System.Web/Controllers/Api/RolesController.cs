using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IRolesService _roleService;

        public RolesController(IRolesService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [HasPermission("ROLE_VIEW")]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Roles/Index.cshtml");
        }

        // GET: /api/roles
        [HttpGet("listRoles")]
        [HasPermission("ROLE_VIEW")]
        public async Task<IActionResult> GetRoles(string? keyword)
        {
            var roles = await _roleService.GetRoles(keyword);
            return Ok(roles);
        }

        // POST: /api/roles
        [HttpPost]
        [HasPermission("ROLE_CREATE")]
        public async Task<IActionResult> CreateRole([FromBody] UserRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.RoleName))
            {
                return BadRequest("Invalid role data");
            }
            var roleId = await _roleService.CreateRole(role);
            return Ok(new { id = roleId });
        }

        // POST: /api/roles/{id}
        [HttpPost("{id}")]
        [HasPermission("ROLE_EDIT")]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UserRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.RoleName))
            {
                return BadRequest("Invalid role data");
            }
            var updatedRole = await _roleService.UpdateRole(id, role);
            return Ok(updatedRole);
        }

        // POST: /api/roles/{id}/status
        [HttpPost("{id}/status")]
        ///[HasPermission("ROLE_TOGGLE_STATUS")]
        [HasPermission("ROLE_EDIT")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var newStatus = await _roleService.ToggleStatus(id);
            if (newStatus == null)
            {
                return NotFound("Role not found");
            }
            return Ok(new { Message = "Status updated", NewStatus = newStatus });
        }

        [HttpPost("delete")]
        [HasPermission("ROLE_DELETE")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _roleService.Delete(id);
            if (!deleted)
            {
                return NotFound("Role not found");
            }


            return Json(new { success = true });
        }
    }
}