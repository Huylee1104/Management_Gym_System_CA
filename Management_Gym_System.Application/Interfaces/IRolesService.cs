using Management_Gym_System.Domain.Entities;

public interface IRolesService
{
    Task<List<UserRole>> GetRoles(string? keyword);
    Task<long?> CreateRole(UserRole role);
    Task<UserRole> UpdateRole(long id,UserRole role);
    Task<bool?> ToggleStatus(long id);
    Task<bool> Delete(long id);
}