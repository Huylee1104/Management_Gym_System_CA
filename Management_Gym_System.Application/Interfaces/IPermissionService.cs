using Management_Gym_System.Domain.Enums;

namespace Management_Gym_System.Application.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(long userId, string functionCode, PermissionType permissionType);
}