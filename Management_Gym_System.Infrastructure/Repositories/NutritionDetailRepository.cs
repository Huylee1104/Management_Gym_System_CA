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

    public async Task<List<NutritionDetail>> GetDetailsByGroupIdAsync(long groupId, bool isAdmin)
    {
        var query = _context.NutritionDetails
        .AsNoTracking()
        .Where(x => x.NutritionGroupId == groupId);

        if (isAdmin == false)
        {
            query = query.Where(d => d.IsPublished == true);
        }

        List<NutritionDetail> details = await query
            .OrderBy(d => d.Name)
            .ToListAsync();

        return details;
    }

    public async Task<NutritionDetail> GetDetailByIdAsync(long IdNutrition)
    {
        var result = await _context.NutritionDetails.
                Include(d => d.NutritionGroup).FirstOrDefaultAsync(x => x.Id == IdNutrition);
        return result ?? new NutritionDetail();
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