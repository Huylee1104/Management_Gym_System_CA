using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface INutritionAnalyzeHistoryRepository
{
    Task AddAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory);
    Task UpdateAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory);
    Task DeleteAsync(NutritionAnalyzeHistory NutritionAnalyzeHistory);
    Task<bool> SaveChangesAsync();
}