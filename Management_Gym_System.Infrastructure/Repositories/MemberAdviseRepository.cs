using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Infrastructure.Repositories;

public class MemberAdviseRepository : IMemberAdviseRepository
{
    private readonly ApplicationDbContext _context;

    public MemberAdviseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MemberAdvise>> GetListAdviseAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.MemberAdvises.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(ma => ma.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(ma => ma.Date < endDate);
        }

        return await query.ToListAsync();
    }

    public async Task<List<MemberAdvise>> GetListAdviseContactedAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.MemberAdvises.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(ma => ma.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);

            query = query.Where(ma => ma.Date < endDate);
        }

        return await query.Where(ma => ma.IsContacted == true).ToListAsync();
    }

    public async Task<MemberAdvise?> GetByIdAsync(long id)
    {
        return await _context.MemberAdvises.FindAsync(id);
    }

    public async Task AddAsync(MemberAdvise memberAdvise)
    {
        await _context.MemberAdvises.AddAsync(memberAdvise);
    }

    public async Task UpdateAsync(MemberAdvise memberAdvise)
    {
        _context.MemberAdvises.Update(memberAdvise);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}