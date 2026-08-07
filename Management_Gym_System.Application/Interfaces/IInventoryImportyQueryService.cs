using Management_Gym_System.Application.DTOs.Inventory;

namespace Management_Gym_System.Application.Interfaces;

public interface IInventoryImportQueryService
{
    Task<List<ImportHistoryItemDto>> GetImportHistorysAsync(DateTime? fromDate, DateTime? toDate, int? productId, string? supplier);
}
