using Management_Gym_System.Application.DTOs.SystemFunction;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemFunction?> GetFunctionByIdAsync(long id)
    {
        return await _context.SystemFunctions
            .Include(x => x.Actions)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddFunctionAsync(SystemFunction function)
    {
        await _context.SystemFunctions.AddAsync(function);
    }

    public Task UpdateFunctionAsync(SystemFunction function)
    {
        _context.SystemFunctions.Update(function);
        return Task.CompletedTask;
    }

    public async Task<List<SystemFunctionAction>> GetActionsAsync(
        long functionId)
    {
        return await _context.SystemFunctionActions
            .Where(x => x.FunctionId == functionId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.DisplayName)
            .ToListAsync();
    }

    public async Task<SystemFunctionAction?> GetActionByIdAsync(long id)
    {
        return await _context.SystemFunctionActions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddActionAsync(SystemFunctionAction action)
    {
        await _context.SystemFunctionActions.AddAsync(action);
    }

    public Task UpdateActionAsync(SystemFunctionAction action)
    {
        _context.SystemFunctionActions.Update(action);
        return Task.CompletedTask;
    }

    public async Task<List<SystemFunction>> GetPermissionTreeAsync(
        long roleId)
    {
        return await _context.SystemFunctions
            .Where(x => x.IsActive)
            .Include(x => x.Actions
                .Where(a => a.IsActive))
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<RolePermission>> GetRolePermissionsAsync(
        long roleId)
    {
        return await _context.RolePermissions
            .Where(x => x.RoleId == roleId)
            .ToListAsync();
    }

    public async Task SaveRolePermissionsAsync(
        long roleId,
        List<RolePermission> permissions)
    {
        var oldPermissions = await _context.RolePermissions
            .Where(x => x.RoleId == roleId)
            .ToListAsync();

        _context.RolePermissions.RemoveRange(oldPermissions);

        await _context.RolePermissions.AddRangeAsync(permissions);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasPermissionAsync(
    long userId,
    string actionCode)
    {
        var roleId = await _context.Users
            .Where(x => x.ID == userId)
            .Select(x => x.RoleID)
            .FirstOrDefaultAsync();

        if (!roleId.HasValue)
            return false;

        return await _context.RolePermissions
            .AnyAsync(x =>
                x.RoleId == roleId.Value &&
                x.IsAllowed &&
                x.Action.IsActive &&
                x.Action.Code == actionCode);
    }

    public async Task<List<string>> GetUserPermissionsAsync(
    long userId)
    {
        var roleId = await _context.Users
            .Where(x => x.ID == userId)
            .Select(x => x.RoleID)
            .FirstOrDefaultAsync();

        if (!roleId.HasValue)
            return new List<string>();

        return await _context.RolePermissions
            .Where(x =>
                x.RoleId == roleId.Value &&
                x.IsAllowed &&
                x.Action.IsActive)
            .Select(x => x.Action.Code)
            .Distinct()
            .ToListAsync();
    }
}