using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IUsersRepository
{
    Task<List<User>> GetAllUsersAsync(string? keyword, long? filterValue);
    Task<User?> GetUserByIdAsync(long id);
    Task<GymMembershipCard?> GetGymMembershipCardByIdAsync();
    Task AddAsync(User user);
    Task AddAsync(GymMembershipCard membershipCard);

    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}