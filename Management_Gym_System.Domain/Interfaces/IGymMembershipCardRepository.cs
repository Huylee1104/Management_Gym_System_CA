using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IGymMembershipCardRepository
{
    Task<GymMembershipCard?> GetByIdAsync(long id);
    Task<List<GymMembershipCard>> GetFilteredCardsAsync(string? filter, string? keyword);
    Task AddRangeAsync(IEnumerable<GymMembershipCard> cards);
    Task<bool> IsRfidExistsAsync(string rfidUid, long excludeId);
    Task AddAsync(GymMembershipCard card);
    Task Update(GymMembershipCard card);
    Task Delete(GymMembershipCard card);
    Task<bool> SaveChangesAsync();
}   