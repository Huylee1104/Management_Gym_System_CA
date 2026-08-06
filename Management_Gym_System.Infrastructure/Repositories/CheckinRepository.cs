using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class CheckinRepository : ICheckinRepository
{
    private readonly ApplicationDbContext _context;

    public CheckinRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Checkin>> GetCheckinsAsync(DateTime? date)
    {
        // 1. Kiểm tra null
        if (!date.HasValue)
        {
            return new List<Checkin>();
        }

        var startDate = date.Value.Date;
        var endDate = startDate.AddDays(1);

        // 3. Truy vấn tối ưu, ngắn gọn
        return await _context.Checkins
            .Where(c => c.Card != null && c.Card.User != null)
            .Include(c => c.Card)
                .ThenInclude(card => card!.User)
            .Where(c => c.CheckinTime >= startDate && c.CheckinTime < endDate)
            .OrderByDescending(c => c.CheckinTime)
            .ToListAsync();
    }

    public async Task<GymMembershipCard?> GetGymMembershipCardAsync(string RFID_UID)
    {
        var card = await _context.GymMembershipCards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.RFID_UID == RFID_UID);

        return card;
    }

    public async Task<GymMembershipCard?> GetGymMembershipCardIdAsync(long cardId)
    {
        var card = await _context.GymMembershipCards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ID == cardId);

        return card;
    }

    public async Task<Checkin> GetCheckinLastDayAsync()
    {
        var today = DateTime.Today;
        var latestCheckin = await _context.Checkins
            .Where(c => c.CheckinTime >= today)
            .OrderByDescending(c => c.CheckinTime)
            .FirstOrDefaultAsync();

        return latestCheckin ?? new Checkin();
    }

    public async Task<bool?> AddTimeCardAsync(long cardId, int ThoiHan)
    {
        var card = await GetGymMembershipCardIdAsync(cardId);
        if (card == null)
        {
            return false;
        }

        var now = DateTime.Now;
        var baseDate = card.EndDate.HasValue && card.EndDate.Value > now
            ? card.EndDate.Value
            : now;

        card.EndDate = baseDate.AddDays(ThoiHan);
        card.Status = true;

        return true;
    }

    public async Task AddAsync(Checkin checkin)
    {
        await _context.Checkins.AddAsync(checkin);
    }

    public async Task Update(Checkin checkin)
    {
        _context.Checkins.Update(checkin);
    }

    public async Task Delete(Checkin checkin)
    {
        _context.Checkins.Remove(checkin);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}