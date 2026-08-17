using Management_Gym_System.Domain.Entities;

public interface IPermissionRepository
{
    Task<SystemFunction?> GetFunctionByIdAsync(long id);

    Task AddFunctionAsync(SystemFunction function);

    Task UpdateFunctionAsync(SystemFunction function);

    Task<List<SystemFunctionAction>> GetActionsAsync(long functionId);

    Task<SystemFunctionAction?> GetActionByIdAsync(long id);

    Task AddActionAsync(SystemFunctionAction action);

    Task UpdateActionAsync(SystemFunctionAction action);

    Task<List<SystemFunction>> GetPermissionTreeAsync(long roleId);

    Task<List<RolePermission>> GetRolePermissionsAsync(long roleId);

    Task SaveRolePermissionsAsync(
        long roleId,
        List<RolePermission> permissions);

    Task<bool> HasPermissionAsync(long userId, string actionCode);
    Task<List<string>> GetUserPermissionsAsync(long userId);

    Task<bool> RoleExistsAsync(long roleId);

    Task<List<long>> GetActiveActionIdsAsync(IEnumerable<long> actionIds);

    Task SaveChangesAsync();
}