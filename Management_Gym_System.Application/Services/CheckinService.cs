using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class CheckinService : ICheckinService
{
    private readonly ICheckinRepository _checkinRepo;

    public CheckinService(ICheckinRepository checkinRepo)
    {
        _checkinRepo = checkinRepo;
    }

    public async Task<List<CheckinDto>> GetCheckinsAsync(DateTime? date)
    {
        var filterDate = date?.Date ?? DateTime.Today;
        var result = await _checkinRepo.GetCheckinsAsync(filterDate);
        return result.Select(c => new CheckinDto
        {
            checkinId = c.ID,
            checkinTime = c.CheckinTime,
            CardID = c.CardID ?? 0,
            fullName = c.Card?.User?.FullName ?? string.Empty,
            avatar = c.Card?.User?.Avatar ?? string.Empty,
            startDate = c.Card?.StartDate,
            endDate = c.Card?.EndDate,
            rfidUid = c.Card?.RFID_UID ?? string.Empty,
            cardStatus = c.Status
        }).ToList();
    }

    public async Task<CardInfo> DoCheckin(string RFID_UID)
    {
        var card = await _checkinRepo.GetGymMembershipCardAsync(RFID_UID);
        if (card == null)
        {
            return new CardInfo();
        }

        var cardStatus = card.Status == false ? "locked"
        : card.EndDate.HasValue && card.EndDate.Value < DateTime.Now ? "expired"
        : "active";

        var checkin = new Checkin
        {
            CardID = card.ID,
            CheckinTime = DateTime.Now,
            Status = cardStatus
        };

        await _checkinRepo.AddAsync(checkin);
        await _checkinRepo.SaveChangesAsync();

        var startDate = card.StartDate.HasValue ? card.StartDate.Value.ToString("dd/MM/yyyy") : null;
        var endDate = card.EndDate.HasValue ? card.EndDate.Value.ToString("dd/MM/yyyy") : null;

        return new CardInfo
        {
            ID = card.ID,
            RfidUid = card.RFID_UID,
            FullName = card.User?.FullName ?? string.Empty,
            PhoneNumber = card.User?.PhoneNumber ?? string.Empty,
            Avatar = card.User?.Avatar ?? string.Empty,
            StartDate = startDate,
            EndDate = endDate,
            CardStatus = cardStatus
        };
    }

    public async Task<DateTime?> ExtendCard(long cardId)
    {
        var card = await _checkinRepo.GetGymMembershipCardIdAsync(cardId);
        if (card == null)
        {
            return null;
        }

        if (!card.EndDate.HasValue || card.EndDate.Value < DateTime.Now)
        {
            return null;
        }

        if (card.Product == null || !card.Product.ThoiHan.HasValue)
        {
            return null;
        }

        int thoiHan = card.Product.ThoiHan.Value;

        var addTime = _checkinRepo.AddTimeCardAsync(cardId, thoiHan);


        card.EndDate = card.EndDate.Value.AddMonths(1);
        await _checkinRepo.SaveChangesAsync();
        return card.EndDate.Value;
    }

    public async Task<bool> LockCard(long cardId)
    {
        var card = await _checkinRepo.GetGymMembershipCardIdAsync(cardId);
        if (card == null)
        {
            return false;
        }

        card.Status = false;
        card.PauseDate = DateTime.Now;

        await _checkinRepo.SaveChangesAsync();
        return true;
    }

    public async Task<DateTime?> UnlockCard(long cardId)
    {
        var card = await _checkinRepo.GetGymMembershipCardIdAsync(cardId);
        if (card == null)
        {
            return null;
        }

        if (card.PauseDate == null)
            return null;
        var resumeDate = DateTime.Now;
        int soNgayTamDung = (int)(resumeDate - card.PauseDate.Value).TotalDays;

        card.ResumeDate = resumeDate;
        card.EndDate = card.EndDate.HasValue
            ? card.EndDate.Value.AddDays(soNgayTamDung)
            : null;
        card.Status = true;

        await _checkinRepo.SaveChangesAsync();
        return card.EndDate;
    }

    public async Task<CardInfo> GetLatestToday()
    {
        var latestCheckin = await _checkinRepo.GetCheckinLastDayAsync();
        if (latestCheckin == null || latestCheckin.Card == null)
        {
            return new CardInfo();
        }

        var card = latestCheckin.Card;
        var cardStatus = card.Status == false ? "locked"
            : card.EndDate.HasValue && card.EndDate.Value < DateTime.Now ? "expired"
            : "active";

        var startDate = card.StartDate.HasValue ? card.StartDate.Value.ToString("dd/MM/yyyy") : null;
        var endDate = card.EndDate.HasValue ? card.EndDate.Value.ToString("dd/MM/yyyy") : null;

        return new CardInfo
        {
            ID = card.ID,
            RfidUid = card.RFID_UID,
            FullName = card.User?.FullName ?? string.Empty,
            PhoneNumber = card.User?.PhoneNumber ?? string.Empty,
            Avatar = card.User?.Avatar ?? string.Empty,
            StartDate = startDate,
            EndDate = endDate,
            CardStatus = cardStatus
        };
    }
}