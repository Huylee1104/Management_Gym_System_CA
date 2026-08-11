using Management_Gym_System.Application.DTOs.Permission;
using Management_Gym_System.Domain.Entities;

public interface IPermissionService
{
    Task<List<SystemFunction>> GetFunctionsAsync();

    Task<long> CreateFunctionAsync(SystemFunction function);

    Task<bool> UpdateFunctionAsync(long id, SystemFunction function);

    Task<long> CreateActionAsync(SystemFunctionAction action);

    Task<bool> UpdateActionAsync(
        long id,
        SystemFunctionAction action);

    Task<PermissionTreeResponse> GetPermissionTreeAsync(long roleId);

    Task SaveRolePermissionsAsync(long roleId, List<PermissionItemRequest> permissions);
    Task<bool> HasPermissionAsync( long userId, string actionCode);
    Task<List<string>> GetUserPermissionsAsync(long userId);
}