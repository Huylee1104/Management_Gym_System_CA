using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [HasPermission("USER_VIEW")]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Users/Index.cshtml");
        }

        [HttpGet("listUsers")]
        [HasPermission("USER_VIEW")]
        public async Task<IActionResult> GetUsers(string? keyword, long? filterValue)
        {
            var users = await _usersService.GetUsers(keyword, filterValue);

            return Ok(users);
        }

        [HttpPost]
        [HasPermission("USER_CREATE")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateUpdateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo user
            var user = await _usersService.CreateUser(request);

            return Ok(new
            {
                success = true,
                data = user
            });
        }

        [HttpPost("{id}")]
        [HasPermission("USER_EDIT")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserCreateUpdateDto request)
        {
            if (id != request.Id)
                return BadRequest();

            var result = await _usersService.UpdateUser(id, request);
            if (!result)
                return NotFound();

            return Ok(new
            {
                success = true
            });
        }

        [HttpPost("{id}/status")]
        [HasPermission("USER_EDIT")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var user = await _usersService.ToggleStatus(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("delete")]
        [HasPermission("USER_DELETE")]
        public async Task<IActionResult> Delete([FromForm]long id)
        {
            var result = await _usersService.Delete(id);
            if (!result)
                return NotFound();

            return Json(new { success = true });
        }
    }
}