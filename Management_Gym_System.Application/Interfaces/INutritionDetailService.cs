using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface INutritionDetailService
{
    Task<List<NutritionDetail>> GetDetailsByGroupIdAsync(long groupId, bool isAdmin);
    Task<NutritionDetail?> GetDetailByIdAsync(long id);
    Task<NutritionDetail> CreateDetailAsync(NutritionDetail model);
    Task<bool> UpdateDetailAsync(NutritionDetail model);
    Task<bool> DeleteDetailAsync(long id);
    
}