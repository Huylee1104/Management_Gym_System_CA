using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class GymMembershipCardService : IGymMembershipCardService
{
    private readonly IGymMembershipCardRepository _cardRepo;

    public GymMembershipCardService(IGymMembershipCardRepository cardRepo)
    {
        _cardRepo = cardRepo;
    }

    public async Task<List<GymMembershipCardDto>> GetFilteredCardsAsync(string? filter, string? keyword)
    {
        var result = await _cardRepo.GetFilteredCardsAsync(filter, keyword);
        return result.Select(c => new GymMembershipCardDto
        {
            ID = c.ID,
            RFID_UID = c.RFID_UID,
            Status = c.Status,
            StartDate = c.StartDate?.ToString("dd-MM-yyyy") ?? "",
            EndDate = c.EndDate?.ToString("dd-MM-yyyy") ?? "",
            PauseDate = c.PauseDate?.ToString("dd-MM-yyyy") ?? "",
            ResumeDate = c.ResumeDate?.ToString("dd-MM-yyyy") ?? "",
            UserName = c.User?.FullName ?? string.Empty,
            ProductName = c.Product?.ProductName ?? string.Empty
        }).ToList();
    }

    public async Task<ServiceResult> CreateCardQualityAsync(int quantity)
    {
        try
        {
            if (quantity <= 0)
        {
            return ServiceResult.Failure("Số lượng thẻ phải lớn hơn 0!");
        }

        var newCards = new List<GymMembershipCard>();
        for (int i = 0; i < quantity; i++)
        {
            newCards.Add(new GymMembershipCard
            {
                RFID_UID = null,
                UserID = null,
                ProductID = null,
                StartDate = null,
                EndDate = null,
                PauseDate = null,
                Status = false
            });
        }

        await _cardRepo.AddRangeAsync(newCards);
        await _cardRepo.SaveChangesAsync();
        return ServiceResult.Success("Tạo thẻ thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Tạo thẻ thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> UpdateCardAsync(long id, string rfidUid)
    {
        try
        {
            var carded = await _cardRepo.GetByIdAsync(id);
            if (carded == null)
            {
                return ServiceResult.Failure("Không tìm thấy thẻ.");
            }

            var existing = await _cardRepo.IsRfidExistsAsync(rfidUid, id);
            if (existing)
            {
                return ServiceResult.Failure("RFID_UID đã tồn tại.");
            }

            carded.RFID_UID = rfidUid;

            await _cardRepo.Update(carded);
            await _cardRepo.SaveChangesAsync();
            return ServiceResult.Success("Cập nhật thẻ thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Cập nhật thẻ thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> LockUnlockCardAsync(long id)
    {
        try
        {
            var card = await _cardRepo.GetByIdAsync(id);
            if (card == null)
            {
                return ServiceResult.Failure("Không tìm thấy thẻ.");
            }

            // Nếu Status đang true -> false. Nếu false hoặc null -> true.
            card.Status = card.Status == true ? false : true;

            await _cardRepo.Update(card);
            await _cardRepo.SaveChangesAsync();
            return ServiceResult.Success("Cập nhật trạng thái thẻ thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Cập nhật trạng thái thẻ thất bại: {ex.Message}");
        }
    }

    public async Task<ServiceResult> DeleteCardAsync(long id)
    {
        try
        {
            var card = await _cardRepo.GetByIdAsync(id);
            if (card == null)
            {
                return ServiceResult.Failure("Không tìm thấy thẻ.");
            }

            await _cardRepo.Delete(card);
            await _cardRepo.SaveChangesAsync();
            return ServiceResult.Success("Đã xóa thẻ thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Xóa thẻ thất bại: {ex.Message}");
        }
    }
}