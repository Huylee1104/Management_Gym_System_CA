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

    public async Task<List<NutritionGroup>> GetAllGroupsAsync(bool isAdmin)
    {
        var result = _context.NutritionGroups.AsNoTracking().AsQueryable();
                // Hội viên chỉ xem được các nhóm đang hoạt động
        if (isAdmin == false)
        {
            result = result.Where(g => g.IsActive == true);
        }

        List<NutritionGroup> groups = await result
            .OrderBy(g => g.SortOrder ?? int.MaxValue)
            .ThenBy(g => g.Name)
            .ToListAsync();
        return groups;
    }

    public async Task<NutritionGroup> GetGroupByIdAsync(long IdGroup)
    {
        var result = await _context.NutritionGroups.
            Include(d => d.NutritionDetails)
            .FirstOrDefaultAsync();
        return result ?? new NutritionGroup();
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

    public async Task DeleteRangeAsync(IEnumerable<NutritionDetail> details)
    {
        _context.RemoveRange(details);
    }
}