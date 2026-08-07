using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class InventoryImportRepository : IInventoryImportRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryImportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ImportReceipt>> GetImportsAsync()
    {
        var imports = await _context.ImportReceipts
            .OrderByDescending(i => i.ImportDate)
            .ToListAsync();
        return imports;
    }

    public async Task AddAsync(ImportReceipt importReceipt)
    {
        await _context.ImportReceipts.AddAsync(importReceipt);
    }
    public async Task AddAsync(FinancialTransaction financialTransaction)
    {
        await _context.FinancialTransactions.AddAsync(financialTransaction);
    }

    public async Task AddAsync(ImportReceiptDetail importReceiptDetail)
    {
        await _context.ImportReceiptDetails.AddAsync(importReceiptDetail);
    }

    public async Task AddAsync(TransactionDetail transactionDetail)
    {
        await _context.TransactionDetails.AddAsync(transactionDetail);
    }

    public async Task AddAsync(InventoryLot inventoryLot)
    {
        await _context.InventoryLots.AddAsync(inventoryLot);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}