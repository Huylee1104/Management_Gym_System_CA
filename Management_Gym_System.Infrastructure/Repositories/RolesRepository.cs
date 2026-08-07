using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class RolesRepository : IRolesRepository
{
    private readonly ApplicationDbContext _context;

    public RolesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserRole>> GetAllRolesAsync(string? keyword = null)
    {
        var query = _context.UserRoles.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            var normalizedKeyword = StringHelper.NormalizeText(keyword);
            query = query.Where(r =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(r.RoleName ?? string.Empty),
                    $"%{normalizedKeyword}%"));
        }

        return await query.ToListAsync();
    }

    public async Task<UserRole?> GetRoleByIdAsync(long id)
    {
        return await _context.UserRoles.FirstOrDefaultAsync(r => r.ID == id);
    }

    public async Task AddAsync(UserRole role)
    {
        await _context.UserRoles.AddAsync(role);
    }

    public async Task UpdateAsync(UserRole role)
    {
        _context.UserRoles.Update(role);
    }

    public async Task DeleteAsync(UserRole role)
    {
        _context.UserRoles.Remove(role);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}