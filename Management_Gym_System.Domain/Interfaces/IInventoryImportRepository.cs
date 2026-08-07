using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IInventoryImportRepository
{
    Task<List<ImportReceipt>> GetImportsAsync();
    Task AddAsync(ImportReceipt importReceipt);
    Task AddAsync(FinancialTransaction financialTransaction);
    Task AddAsync(ImportReceiptDetail importReceiptDetail);
    Task AddAsync(TransactionDetail transactionDetail);
    Task AddAsync(InventoryLot inventoryLot);
    Task<bool> SaveChangesAsync();
}