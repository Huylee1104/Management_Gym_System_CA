using Management_Gym_System.Application.DTOs.Permission;
using Management_Gym_System.Application.DTOs.SystemFunction;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Entities;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _repository;
    private readonly ISystemFunctionQueryService _systemFunctionQueryService;

    public PermissionService(IPermissionRepository repository, ISystemFunctionQueryService systemFunctionQueryService)
    {
        _repository = repository;
        _systemFunctionQueryService = systemFunctionQueryService;
    }

    public async Task<List<SystemFunctionDto>> GetFunctionsAsync()
    {
        return await _systemFunctionQueryService.GetFunctionsAsync();
    }

    public async Task<long> CreateFunctionAsync(
        SystemFunction function)
    {
        await _repository.AddFunctionAsync(function);
        await _repository.SaveChangesAsync();

        return function.Id;
    }

    public async Task<bool> UpdateFunctionAsync(
        long id,
        SystemFunction function)
    {
        var existing =
            await _repository.GetFunctionByIdAsync(id);

        if (existing == null)
            return false;

        existing.Code = function.Code;
        existing.Name = function.Name;
        existing.Controller = function.Controller;
        existing.Description = function.Description;
        existing.IsActive = function.IsActive;
        existing.DisplayOrder = function.DisplayOrder;

        await _repository.UpdateFunctionAsync(existing);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<long> CreateActionAsync(
        SystemFunctionAction action)
    {
        await _repository.AddActionAsync(action);
        await _repository.SaveChangesAsync();

        return action.Id;
    }

    public async Task<bool> UpdateActionAsync(
        long id,
        SystemFunctionAction action)
    {
        var existing =
            await _repository.GetActionByIdAsync(id);

        if (existing == null)
            return false;

        existing.FunctionId = action.FunctionId;
        existing.Code = action.Code;
        existing.ActionName = action.ActionName;
        existing.DisplayName = action.DisplayName;
        existing.Description = action.Description;
        existing.IsActive = action.IsActive;
        existing.DisplayOrder = action.DisplayOrder;

        await _repository.UpdateActionAsync(existing);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<PermissionTreeResponse>
        GetPermissionTreeAsync(long roleId)
    {
        var functions =
            await _repository.GetPermissionTreeAsync(roleId);

        var rolePermissions =
            await _repository.GetRolePermissionsAsync(roleId);

        var allowedActionIds = rolePermissions
            .Where(x => x.IsAllowed)
            .Select(x => x.ActionId)
            .ToHashSet();

        return new PermissionTreeResponse
        {
            RoleId = roleId,

            Functions = functions.Select(f => new PermissionFunctionDto
            {
                Id = f.Id,
                Code = f.Code,
                Name = f.Name,

                Actions = f.Actions.Select(a =>
                    new PermissionActionDto
                    {
                        Id = a.Id,
                        Code = a.Code,
                        ActionName = a.ActionName,
                        DisplayName = a.DisplayName,
                        IsAllowed = allowedActionIds.Contains(a.Id)
                    }).ToList()

            }).ToList()
        };
    }

    public async Task SaveRolePermissionsAsync(
        long roleId,
        List<PermissionItemRequest> permissions)
    {
        var entities = permissions.Select(x =>
            new RolePermission
            {
                RoleId = roleId,
                ActionId = x.ActionId,
                IsAllowed = x.IsAllowed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

        await _repository.SaveRolePermissionsAsync(
            roleId,
            entities);

        await _repository.SaveChangesAsync();
    }

    public async Task<bool> HasPermissionAsync(
    long userId,
    string actionCode)
    {
        if (string.IsNullOrWhiteSpace(actionCode))
            return false;

        return await _repository.HasPermissionAsync(
            userId,
            actionCode);
    }

    public async Task<List<string>> GetUserPermissionsAsync(
    long userId)
    {
        return await _repository.GetUserPermissionsAsync(userId);
    }
}
