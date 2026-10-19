using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface INutritionGroupRepository
{
    Task<List<NutritionGroup>> GetAllGroupsAsync(bool isAdmin);
    Task<NutritionGroup> GetGroupByIdAsync(long IdGroup);
    Task AddAsync(NutritionGroup NutritionGroup);
    Task UpdateAsync(NutritionGroup NutritionGroup);
    Task DeleteAsync(NutritionGroup NutritionGroup);
    Task DeleteRangeAsync(IEnumerable<NutritionDetail> details);
    Task<bool> SaveChangesAsync();
}