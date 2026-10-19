using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class NutritionDetailRepository : INutritionDetailRepository
{
    private readonly ApplicationDbContext _context;

    public NutritionDetailRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NutritionDetail NutritionDetail)
    {
        await _context.NutritionDetails.AddAsync(NutritionDetail);
    }

    public async Task UpdateAsync(NutritionDetail NutritionDetail)
    {
        _context.NutritionDetails.Update(NutritionDetail);
    }

    public async Task DeleteAsync(NutritionDetail NutritionDetail)
    {
        _context.NutritionDetails.Remove(NutritionDetail);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}