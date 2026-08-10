using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Enums;
using Management_Gym_System.Domain.Interfaces;

namespace Management_Gym_System.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IRolePermissionRepository _permissionRepository;
    private readonly IUsersRepository _usersRepository;

    public PermissionService(IRolePermissionRepository permissionRepository, IUsersRepository usersRepository)
    {
        _permissionRepository = permissionRepository;
        _usersRepository = usersRepository;
    }

    public async Task<bool> HasPermissionAsync(long userId, string functionCode, PermissionType permissionType)
    {
        var user = await _usersRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var permission = await _permissionRepository.GetRolePermissionAsync(userId, functionCode, user);

        if (permission == null)
        {
            return false;
        }

        return permissionType switch
        {
            PermissionType.View => permission.CanView,
            PermissionType.Create => permission.CanCreate,
            PermissionType.Edit => permission.CanEdit,
            PermissionType.Delete => permission.CanDelete,
            PermissionType.Export => permission.CanExport,
            _ => false
        };
    }
}