using Management_Gym_System.Application.DTOs.Inventory;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

namespace Management_Gym_System.Application.Services;

public class InventoryImportService : IInventoryImportService
{
    private readonly IInventoryImportRepository _inventoryImportRepository;
    private readonly IInventoryQueryService _inventoryQueryService;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryImportService(IInventoryImportRepository inventoryImportRepository, IInventoryQueryService inventoryQueryService, IUnitOfWork unitOfWork)
    {
        _inventoryImportRepository = inventoryImportRepository;
        _inventoryQueryService = inventoryQueryService;
        _unitOfWork = unitOfWork;
    }
    public async Task<ServiceResultWithId> CreateImport(ImportRequestDto request)
    {
        if (request.Details == null || !request.Details.Any())
            return ServiceResultWithId.Failure("Không có chi tiết phiếu nhập.");

        await _unitOfWork.BeginTransactionAsync();
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
                await _inventoryImportRepository.AddAsync(importReceipt);
                await _inventoryImportRepository.SaveChangesAsync();

                var financialTransaction = new FinancialTransaction // Giao dịch tài chính tương ứng
                {
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = true,
                    StaffID = 3,
                    Note = $"Chi tiền nhập hàng phiếu #{importReceipt.ID}. NCC: {request.Supplier}",
                    TotalAmount = 0
                };
                await _inventoryImportRepository.AddAsync(financialTransaction);
                await _inventoryImportRepository.SaveChangesAsync();

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
                    await _inventoryImportRepository.AddAsync(importDetail);
                    await _inventoryImportRepository.SaveChangesAsync();

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
                    await _inventoryImportRepository.AddAsync(inventoryLot);
                    await _inventoryImportRepository.SaveChangesAsync();

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
                    await _inventoryImportRepository.AddAsync(finDetail);
                }

                importReceipt.TotalAmount = totalFinal;
                financialTransaction.TotalAmount = totalFinal;

                await _inventoryImportRepository.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return ServiceResultWithId.Success("Phiếu nhập kho đã được tạo thành công.", importReceipt.ID);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResultWithId.Failure(ex.Message);
            }
    }

    public async Task<List<ImportHistoryItemDto>> GetImportHistory(ImportHistoryFilterDto filter)
    {
        DateTime? fromDate = string.IsNullOrEmpty(filter.FromDate) ? null : DateTime.Parse(filter.FromDate);
        DateTime? toDate = string.IsNullOrEmpty(filter.ToDate) ? null : DateTime.Parse(filter.ToDate).AddDays(1);

        return await _inventoryQueryService.GetImportHistorysAsync(fromDate, toDate, filter.ProductId, filter.Supplier);
    }
}