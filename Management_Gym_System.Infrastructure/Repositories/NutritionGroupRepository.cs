using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class NutritionGroupRepository : INutritionGroupRepository
{
    private readonly ApplicationDbContext _context;

    public NutritionGroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NutritionGroup NutritionGroup)
    {
        await _context.NutritionGroups.AddAsync(NutritionGroup);
    }

    public async Task UpdateAsync(NutritionGroup NutritionGroup)
    {
        _context.NutritionGroups.Update(NutritionGroup);
    }

    public async Task DeleteAsync(NutritionGroup NutritionGroup)
    {
        _context.NutritionGroups.Remove(NutritionGroup);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}