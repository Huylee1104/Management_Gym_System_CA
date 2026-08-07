using Management_Gym_System.Application.DTOs.Inventory;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dapper;

namespace Management_Gym_System.Infrastructure.Queries;

public class InventoryImportQueryService : IInventoryImportQueryService
{
    private readonly ApplicationDbContext _context;

    public InventoryImportQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ImportHistoryItemDto>> GetImportHistorysAsync(DateTime? fromDate, DateTime? toDate, int? productId, string? supplier)
    {
        try
        {
            using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());

            var result = await connection.QueryAsync<ImportHistoryItemDto>(
                "SELECT * FROM get_import_history(@fromDate, @toDate, @productId, @supplier)",
                new
                {
                    fromDate = fromDate,
                    toDate = toDate,
                    productId = productId,
                    supplier = supplier
                }
            );

            return result.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching import history: {ex.Message}");
            throw;
        }
    }
}