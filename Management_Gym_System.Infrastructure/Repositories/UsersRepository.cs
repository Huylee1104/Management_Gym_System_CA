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

    public async Task<List<User>> GetAllUsersAsync()
    {
        var query = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Memberships)
            .ThenInclude(m => m.Product)
            .ToListAsync();

        return query;
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

    public async Task UpdateAsync(GymMembershipCard membershipCard)
    {
         _context.GymMembershipCards.Update(membershipCard);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<bool> UpdateGymMembershipCard(long id)
    {
        var card = await _context.GymMembershipCards.FirstOrDefaultAsync(x => x.UserID == id);
        if (card == null)
        {
            return false;
        }
        card.UserID = null;
        card.ProductID = null;
        card.StartDate = null;
        card.EndDate = null;
        card.PauseDate = null;
        card.ResumeDate = null;
        _context.Update(card);
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool?> GetExistingUser(string username)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (existingUser != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}