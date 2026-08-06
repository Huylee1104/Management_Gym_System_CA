using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface ICheckinRepository
{
    Task<List<Checkin>> GetCheckinsAsync(DateTime? date);
    Task<GymMembershipCard?> GetGymMembershipCardAsync(string RFID_UID);
    Task<GymMembershipCard?> GetGymMembershipCardIdAsync(long cardId);
    Task<Checkin> GetCheckinLastDayAsync();
    Task<bool?> AddTimeCardAsync(long cardId, int ThoiHan);
    Task AddAsync(Checkin card);
    Task Update(Checkin card);
    Task Delete(Checkin card);
    Task<bool> SaveChangesAsync();
}