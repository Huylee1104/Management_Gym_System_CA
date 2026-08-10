using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IRolePermissionRepository
{
    Task<RolePermission> GetRolePermissionAsync(long userId, string functionCode, User user);
}