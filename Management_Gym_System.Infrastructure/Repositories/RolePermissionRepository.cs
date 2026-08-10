using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _context;

    public RolePermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RolePermission> GetRolePermissionAsync(long userId, string functionCode, User user)
    {
        var permission = await _context.RolePermissions
            .AsNoTracking()
            .Include(rp => rp.Function)
            .FirstOrDefaultAsync(rp => rp.RoleId == user.RoleID
                                    && rp.Function.Code == functionCode
                                    && rp.Function.IsActive);

        return permission ?? new RolePermission();
    }
}