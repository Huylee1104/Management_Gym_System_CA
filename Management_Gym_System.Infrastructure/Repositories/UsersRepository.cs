using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext _context;

    public UsersRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync(string? keyword, long? filterValue)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .Include(u => u.Memberships)
                .ThenInclude(m => m.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = StringHelper.NormalizeText(keyword);
            query = query.Where(u =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(u.FullName ?? string.Empty),
                    $"%{normalizedKeyword}%"));
        }

        if (filterValue.HasValue)
        {
            query = query.Where(u =>
                u.Memberships.Any(m => m.ProductID == filterValue.Value));
        }

        return await query.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<GymMembershipCard?> GetGymMembershipCardByIdAsync()
    {
        return await _context.GymMembershipCards.FirstOrDefaultAsync(x =>
                        x.UserID == null &&
                        !string.IsNullOrEmpty(x.RFID_UID) &&
                        x.Status == true);
    }


    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task AddAsync(GymMembershipCard membershipCard)
    {
        await _context.GymMembershipCards.AddAsync(membershipCard);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}