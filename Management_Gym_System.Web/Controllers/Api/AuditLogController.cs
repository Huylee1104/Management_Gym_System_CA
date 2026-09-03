using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Web.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        //[HasPermission("AUDITLOG_VIEW")]
        public IActionResult Index()
        {
            return View("~/Views/AuditLog/Index.cshtml");
        }

        [HttpGet("GetList")]
        public async Task<IActionResult> GetActivityLogs([FromQuery] ActivityLogRequest request)
        {
            var response = await _auditLogService.GetActivityLogsAsync(request);
            return Ok(response);
        }
    }
}