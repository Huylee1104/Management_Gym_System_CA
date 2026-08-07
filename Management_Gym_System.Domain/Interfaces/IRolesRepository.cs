using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IRolesRepository
{
    Task<List<UserRole>> GetAllRolesAsync(string? keyword = null);
    Task<UserRole?> GetRoleByIdAsync(long id);
    Task AddAsync(UserRole role);
    Task UpdateAsync(UserRole role);
    Task DeleteAsync(UserRole role);
    Task SaveChangesAsync();
}