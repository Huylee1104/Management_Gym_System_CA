using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class GymMembershipCardRepository : IGymMembershipCardRepository
{
    private readonly ApplicationDbContext _context;

    public GymMembershipCardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GymMembershipCard?> GetByIdAsync(long id)
    {
        return await _context.GymMembershipCards.FindAsync(id);
    }

    public async Task<List<GymMembershipCard>> GetFilteredCardsAsync(string? filter, string? keyword)
    {
        var query = _context.GymMembershipCards.AsQueryable();

        switch (filter)
            {
                case "unregistered":
                    query = query.Where(c => string.IsNullOrEmpty(c.RFID_UID));
                    break;
                case "active":
                    query = query.Where(c => c.Status == true);
                    break;
                case "inactive":
                    query = query.Where(c => c.Status == false && !string.IsNullOrEmpty(c.RFID_UID));
                    break;
                case "all":
                default:
                    break;
            }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = StringHelper.NormalizeText(keyword);
            query = query.Where(c =>
                EF.Functions.ILike(
                    EF.Functions.Unaccent(c.RFID_UID ?? string.Empty),
                    $"%{normalizedKeyword}%"));
        }

        return await query.OrderBy(c => c.ID).ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<GymMembershipCard> cards)
    {
        await _context.GymMembershipCards.AddRangeAsync(cards);
    }

    public async Task<bool> IsRfidExistsAsync(string rfidUid, long excludeId)
    {
        return await _context.GymMembershipCards
            .AnyAsync(c => c.RFID_UID == rfidUid && c.ID != excludeId);
    }

    public async Task AddAsync(GymMembershipCard card)
    {
        await _context.GymMembershipCards.AddAsync(card);
    }

    public async Task Update(GymMembershipCard card)
    {
        _context.GymMembershipCards.Update(card);
    }

    public async Task Delete(GymMembershipCard card)
    {
        _context.GymMembershipCards.Remove(card);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}