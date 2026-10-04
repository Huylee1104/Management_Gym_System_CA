using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IUsersRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<bool?> GetExistingUser(string username);
    Task<GymMembershipCard?> GetGymMembershipCardByIdAsync();
    Task AddAsync(User user);
    Task UpdateAsync(GymMembershipCard membershipCard);

    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> UpdateGymMembershipCard(long id);
    Task SaveChangesAsync();
}