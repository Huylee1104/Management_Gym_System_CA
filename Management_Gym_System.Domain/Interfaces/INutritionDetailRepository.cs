using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface INutritionDetailRepository
{
    Task<List<NutritionDetail>> GetDetailsByGroupIdAsync(long GroupId, bool isAdmin);
    Task<NutritionDetail> GetDetailByIdAsync(long IdNutrition);
    Task AddAsync(NutritionDetail NutritionDetail);
    Task UpdateAsync(NutritionDetail NutritionDetail);
    Task DeleteAsync(NutritionDetail NutritionDetail);
    Task<bool> SaveChangesAsync();
}