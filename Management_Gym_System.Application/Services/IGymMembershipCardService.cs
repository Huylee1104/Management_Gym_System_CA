using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IGymMembershipCardService
{
    Task<List<GymMembershipCardDto>> GetFilteredCardsAsync(string? filter, string? keyword);
    Task<ServiceResult> CreateCardQualityAsync(int quantity);
    Task<ServiceResult> UpdateCardAsync(long id, string rfidUid);
    Task<ServiceResult> LockUnlockCardAsync(long id);
    Task<ServiceResult> DeleteCardAsync(long id);
}