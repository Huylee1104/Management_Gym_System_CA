using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Application.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;
using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Web.Authorization;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class InventoryImportController : Controller
    {
        private readonly IInventoryImportService _inventoryImportService;
        private readonly IInventoryImportRepository _inventoryImportRepository;

        public InventoryImportController(IInventoryImportService inventoryImportService, IInventoryImportRepository inventoryImportRepository)
        {
            _inventoryImportService = inventoryImportService;
            _inventoryImportRepository = inventoryImportRepository;
        }

        [HttpGet]
        [HasPermission("INVENTORYIMPORT_VIEW")]
        public IActionResult Index() => View("~/Views/Imports/Index.cshtml");

        [HttpGet("listImports")]
        [HasPermission("INVENTORYIMPORT_VIEW")]
        public async Task<IActionResult> GetImports()
        {
            return Ok(await _inventoryImportRepository.GetImportsAsync());
        }

        #region Tạo phiếu nhập kho mới
        [HttpPost]
        [HasPermission("INVENTORYIMPORT_CREATE")]
        public async Task<IActionResult> CreateImport([FromBody] ImportRequestDto request)
        {
            var result = await _inventoryImportService.CreateImport(request);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Nhập kho thành công!", importId = result.Id });
        }
        #endregion

        #region Lấy lịch sử phiếu nhập kho
        [HttpGet("history")]
        [HasPermission("INVENTORYIMPORT_VIEW")]
        public async Task<IActionResult> GetImportHistory([FromQuery] ImportHistoryFilterDto filter)
        {
            try
            {
                var history = await _inventoryImportService.GetImportHistory(filter);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        #endregion
    }
}