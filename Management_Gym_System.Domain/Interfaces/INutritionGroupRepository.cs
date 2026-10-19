using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface INutritionGroupRepository
{
    Task AddAsync(NutritionGroup NutritionGroup);
    Task UpdateAsync(NutritionGroup NutritionGroup);
    Task DeleteAsync(NutritionGroup NutritionGroup);
    Task<bool> SaveChangesAsync();
}