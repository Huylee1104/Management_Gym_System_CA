using Management_Gym_System.Application.DTOs.Inventory;
using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IInventoryImportService
{
    Task<ServiceResultWithId> CreateImport(ImportRequestDto request);
    Task<List<ImportHistoryItemDto>> GetImportHistory(ImportHistoryFilterDto filter);
}
