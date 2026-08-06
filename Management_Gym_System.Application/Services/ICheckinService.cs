using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface ICheckinService
{
    public Task<List<CheckinDto>> GetCheckinsAsync(DateTime? date);
    public Task<CardInfo> DoCheckin(string RFID_UID);
    public Task<DateTime?> ExtendCard(long cardId);
    public Task<bool> LockCard(long cardId);
    public Task<DateTime?> UnlockCard(long cardId);
    public Task<CardInfo> GetLatestToday();
}