using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface INutritionGroupService
{
    Task<IReadOnlyList<NutritionGroup>> GetAllGroupsAsync(bool isAdmin);
    Task<NutritionGroup?> GetGroupByIdAsync(long id);
    Task<NutritionGroup> CreateGroupAsync(NutritionGroup model);
    Task<bool> UpdateGroupAsync(NutritionGroup model);
    Task<bool> DeleteGroupAsync(long id);
    
}