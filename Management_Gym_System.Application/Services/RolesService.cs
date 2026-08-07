using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class RolesService : IRolesService
{
    private readonly IRolesRepository _rolesRepo;

    public RolesService(IRolesRepository rolesRepo)
    {
        _rolesRepo = rolesRepo;
    }

    public async Task<List<UserRole>> GetRoles(string? keyword)
    {
        return await _rolesRepo.GetAllRolesAsync(keyword);
    }

    public async Task<long?> CreateRole(UserRole role)
    {
        await _rolesRepo.AddAsync(role);
        await _rolesRepo.SaveChangesAsync();
        return role.ID;
    }

    public async Task<UserRole> UpdateRole(long id, UserRole role)
    {
        var existingRole = await _rolesRepo.GetRoleByIdAsync(id);
        if (existingRole == null) throw new InvalidOperationException("Role not found");

        existingRole.RoleName = role.RoleName;
        existingRole.Status = role.Status;

        await _rolesRepo.UpdateAsync(existingRole);
        await _rolesRepo.SaveChangesAsync();
        return existingRole;
    }

    public async Task<bool?> ToggleStatus(long id)
    {
        var existingRole = await _rolesRepo.GetRoleByIdAsync(id);
        if (existingRole == null) throw new InvalidOperationException("Role not found");

        existingRole.Status = !existingRole.Status;
        await _rolesRepo.UpdateAsync(existingRole);
        await _rolesRepo.SaveChangesAsync();
        return existingRole.Status;
    }

    public async Task<bool> Delete(long id)
    {
        var role = await _rolesRepo.GetRoleByIdAsync(id);
        if (role == null) return false;

        await _rolesRepo.DeleteAsync(role);
        await _rolesRepo.SaveChangesAsync();
        return true;
    }
}