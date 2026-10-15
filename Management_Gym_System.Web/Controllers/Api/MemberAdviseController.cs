using System.Drawing;
using Management_Gym_System.Application.DTOs.MemberAdvise;
using Management_Gym_System.Application.Services;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class MemberAdviseController : Controller
    {
        public readonly IMemberAdviseService _memberAdviseService;
        public MemberAdviseController(IMemberAdviseService memberAdviseService)
        {
            _memberAdviseService = memberAdviseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View("~/Views/MemberAdvise/Index.cshtml");
        }

        [HttpPost("ExportPdf")]
        public async Task<IActionResult> ExportPdf([FromBody] AdviseFilterRequest filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }
            var pdfBytes = await _memberAdviseService.ExportPdfAsync(filter);
            string fileName = $"DanhSachKhachHangCanTuVan_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // ================= 3. XUẤT EXCEL (EPPlus) =================
        [HttpPost("ExportExcel")]
        public async Task<IActionResult> ExportExcel([FromBody] AdviseFilterRequest filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }
            var excelBytes = await _memberAdviseService.ExportExcelAsync(filter);
            string fileName = $"DanhSachKhachHangCanTuVan_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("GetList")]
        public async Task<List<AdviseCustomerDto>> QueryAdviseDataAsync([FromBody]AdviseFilterRequest filter)
        {
            var result = await _memberAdviseService.GetListMemberAdviseAsync(filter);
            return result ?? new List<AdviseCustomerDto>();
        }
    }
}