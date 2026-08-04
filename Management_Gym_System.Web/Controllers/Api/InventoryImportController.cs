using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Application.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Npgsql;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/inventory/import")]
    [ApiController]
    public class InventoryImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() => View("~/Views/Imports/Index.cshtml");

        [HttpGet("listImports")]
        public async Task<IActionResult> GetImports()
        {
            var imports = await _context.ImportReceipts
                .OrderByDescending(i => i.ImportDate)
                .ToListAsync();
            return Ok(imports);
        }

        #region Tạo phiếu nhập kho mới
        [HttpPost]
        public async Task<IActionResult> CreateImport([FromBody] ImportRequestDto request)
        {
            if (request.Details == null || !request.Details.Any())
                return BadRequest("Dữ liệu không hợp lệ.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalFinal = 0;

                var importReceipt = new ImportReceipt // Phiếp nhập kho
                {
                    ImportDate = DateTime.UtcNow,
                    StaffID = 3,
                    SupplySource = request.Supplier,
                    TotalAmount = 0
                };
                _context.ImportReceipts.Add(importReceipt);
                await _context.SaveChangesAsync();

                var financialTransaction = new FinancialTransaction // Giao dịch tài chính tương ứng
                {
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = true,
                    StaffID = 3,
                    Note = $"Chi tiền nhập hàng phiếu #{importReceipt.ID}. NCC: {request.Supplier}",
                    TotalAmount = 0
                };
                _context.FinancialTransactions.Add(financialTransaction);
                await _context.SaveChangesAsync();

                foreach (var item in request.Details)
                {
                    decimal lineOrigin = item.Quantity * item.Price;
                    decimal afterDisc = lineOrigin - (lineOrigin * item.Discount / 100m);
                    decimal lineFinal = afterDisc + (afterDisc * item.TaxRate / 100m);
                    totalFinal += lineFinal;

                    // Này là phiếp nhập kho chi tiết
                    var importDetail = new ImportReceiptDetail
                    {
                        ImportReceiptID = importReceipt.ID,
                        ProductID = item.ProductId,
                        BatchCode = item.BatchCode,
                        ExpiryDate = string.IsNullOrEmpty(item.ExpiryDate) ? null : DateTime.Parse(item.ExpiryDate),
                        Quantity = (int)item.Quantity,
                        ImportPrice = item.Price,
                        Discount = item.Discount,
                        TaxRate = item.TaxRate,
                        TotalDiscount = lineOrigin * item.Discount / 100m,
                        TotalTax = afterDisc * item.TaxRate / 100m
                    };
                    _context.ImportReceiptDetails.Add(importDetail);
                    await _context.SaveChangesAsync();

                    // Tạo lô tồn kho
                    var inventoryLot = new InventoryLot
                    {
                        ProductId = item.ProductId,
                        ImportDetailId = importDetail.ID,
                        BatchCode = item.BatchCode,
                        ExpiryDate = string.IsNullOrEmpty(item.ExpiryDate) ? null : DateTime.Parse(item.ExpiryDate),
                        UnitCost = afterDisc / item.Quantity, // Giá sau chiết khấu / số lượng
                        OriginalQuantity = (int)item.Quantity,
                        CurrentQuantity = (int)item.Quantity,
                        ReservedQuantity = 0,
                        Status = "active",
                        CreatedAt = DateTime.Now
                    };
                    _context.InventoryLots.Add(inventoryLot);
                    await _context.SaveChangesAsync();

                    // Này là chi tiết giao dịch tài chính
                    var finDetail = new TransactionDetail
                    {
                        TransactionID = financialTransaction.ID,
                        ProductID = item.ProductId,
                        Quantity = (int)item.Quantity,
                        UnitPrice = item.Price,
                        DiscountRare = item.Discount,
                        VATRare = item.TaxRate,
                        TotalDiscount = lineOrigin * item.Discount / 100m,
                        TotalVAT = afterDisc * item.TaxRate / 100m,  // fix: tính VAT sau chiết khấu
                        SubTotal = lineFinal
                    };
                    _context.TransactionDetails.Add(finDetail);
                }

                importReceipt.TotalAmount = totalFinal;
                financialTransaction.TotalAmount = totalFinal;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Nhập kho thành công!", importId = importReceipt.ID });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Lấy lịch sử phiếu nhập kho
        [HttpGet("history")]
        public async Task<IActionResult> GetImportHistory([FromQuery] ImportHistoryFilterDto filter)
        {
            try
            {
                DateTime? fromDate = string.IsNullOrEmpty(filter.FromDate) ? null : DateTime.Parse(filter.FromDate);
                DateTime? toDate = string.IsNullOrEmpty(filter.ToDate) ? null : DateTime.Parse(filter.ToDate).AddDays(1);

                using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

                var result = await connection.QueryAsync<ImportHistoryItemDto>(
                    "SELECT * FROM get_import_history(@fromDate, @toDate, @productId, @supplier)",
                    new
                    {
                        fromDate,
                        toDate,
                        productId = filter.ProductId,
                        supplier = filter.Supplier
                    }
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        #endregion
    }
}