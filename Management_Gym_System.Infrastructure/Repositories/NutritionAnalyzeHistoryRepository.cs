using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class NutritionAnalyzeHistoryRepository : INutritionAnalyzeHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public NutritionAnalyzeHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory)
    {
        await _context.NutritionAnalyzeHistorys.AddAsync(NutritionAnalyzeHistory);
    }

    public async Task UpdateAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory)
    {
        _context.NutritionAnalyzeHistorys.Update(NutritionAnalyzeHistory);
    }

    public async Task DeleteAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory)
    {
        _context.NutritionAnalyzeHistorys.Remove(NutritionAnalyzeHistory);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}