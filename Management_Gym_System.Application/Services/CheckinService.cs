using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class CheckinService : ICheckinService
{
    private readonly ICheckinRepository _checkinRepo;
    private readonly IMembershipAuditLogRepository _auditlog;
    private readonly IPermissionService _permission;


    public CheckinService(ICheckinRepository checkinRepo, IMembershipAuditLogRepository auditlog, IPermissionService permission)
    {
        _checkinRepo = checkinRepo;
        _auditlog = auditlog;
        _permission = permission;
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

        return new CardInfo
        {
            ID = card.ID,
            RfidUid = card.RFID_UID,
            FullName = card.User?.FullName ?? string.Empty,
            PhoneNumber = card.User?.PhoneNumber ?? string.Empty,
            Avatar = card.User?.Avatar ?? string.Empty,
            StartDate = card.StartDate,
            EndDate = card.EndDate,
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

        var addTime = await _checkinRepo.AddTimeCardAsync(card, thoiHan);

        if (addTime != true)
        {
            return null;
        }

        var idStaff = _permission.GetUserId();

        var changes = new List<object>();

        if (card.StartDate != null)     
        {
            changes.Add(new
            {
                Field = "",
                Display = "Gia hạn thẻ",
                Old = "Ngày bắt đầu: " + card.StartDate + "\nNgày gia hạn: " + DateTime.UtcNow,
                New = "Ngày hết hạn: " + card.EndDate
            });
        }

        var audit = new MembershipAuditLog
        {
            Date = DateTime.UtcNow,
            StaffId = idStaff,
            Action = "Extend",
            MemberId = card.UserID ?? null,
            Note = "Đã gia hạn cho hội viên: " + card.User?.FullName ?? string.Empty,
            DataEdited = System.Text.Json.JsonSerializer.Serialize(changes)
        };

        await _auditlog.AddAsync(audit);
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
        card.PauseDate = DateTime.UtcNow;

        var idStaff = _permission.GetUserId();

        var changes = new List<object>();

        if (card.StartDate != null)
        {
            changes.Add(new
            {
                Field = "",
                Display = "Ngày khóa thẻ",
                Old = "Ngày bắt đầu: " + card.StartDate,
                New = "Ngày khóa: " + DateTime.UtcNow
            });
        }

        var audit = new MembershipAuditLog
        {
            Date = DateTime.UtcNow,
            StaffId = idStaff,
            Action = "Lock",
            MemberId = card.UserID ?? null,
            Note = "Đã khóa thẻ hội viên: " + card.User?.FullName ?? string.Empty,
            DataEdited = System.Text.Json.JsonSerializer.Serialize(changes)
        };

        await _auditlog.AddAsync(audit);
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

        var idStaff = _permission.GetUserId();

        var changes = new List<object>();

        if (card.PauseDate != null)
        {
            changes.Add(new
            {
                Field = "",
                Display = "Ngày mở thẻ",
                Old = "Ngày bắt đầu: " + card.StartDate + "\nNgày khóa: " + card.PauseDate + "\nSố ngày tạm dừng: " + soNgayTamDung,
                New = "Ngày hết hạn: " + card.EndDate
            });
        }

        var audit = new MembershipAuditLog
        {
            Date = DateTime.UtcNow,
            StaffId = idStaff,
            Action = "UnLock",
            MemberId = card.UserID ?? null,
            Note = "Đã mở khóa thẻ hội viên: " + card.User?.FullName ?? string.Empty,
            DataEdited = System.Text.Json.JsonSerializer.Serialize(changes)
        };

        await _auditlog.AddAsync(audit);

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


        return new CardInfo
        {
            ID = card.ID,
            RfidUid = card.RFID_UID,
            FullName = card.User?.FullName ?? string.Empty,
            PhoneNumber = card.User?.PhoneNumber ?? string.Empty,
            Avatar = card.User?.Avatar ?? string.Empty,
            StartDate = card.StartDate,
            EndDate = card.EndDate,
            CardStatus = cardStatus
        };
    }
}