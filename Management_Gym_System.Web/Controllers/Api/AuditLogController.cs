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
        [HasPermission("AUDITLOG_VIEW")]
        public IActionResult Index()
        {
            return View("~/Views/AuditLog/Index.cshtml");
        }

        [HttpGet("GetList")]
        [HasPermission("AUDITLOG_VIEW")]
        public async Task<IActionResult> GetActivityLogs([FromQuery] ActivityLogRequest request)
        {
            var response = await _auditLogService.GetActivityLogsAsync(request);
            return Ok(response);
        }

        [HttpGet("ExportExcel")]
        [HasPermission("AUDITLOG_VIEW")]
        public async Task<IActionResult> ExportExcel([FromQuery]ActivityLogRequest request)
        {
            var file = await _auditLogService.GetExportExcelLogsAsync(request);

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BaoCaoLichSuHoatDong_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpGet("ExportPdf")]
        [HasPermission("AUDITLOG_VIEW")]
        public async Task<IActionResult> ExportPdf([FromQuery]ActivityLogRequest request)
        {
            var file = await _auditLogService.GetExportPdfLogsAsync(request);

            return File(
                file,
                "application/pdf",
                $"BaoCaoLichSuHoatDong_{DateTime.Now:yyyyMMddHHmmss}.pdf"
            );
        }
    }
}