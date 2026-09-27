using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MembershipAuditLogRepository : IMembershipAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public MembershipAuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MembershipAuditLog audit)
    {
        await _context.MembershipAuditLogs.AddAsync(audit);
    }

    public async Task Update(MembershipAuditLog audit)
    {
        _context.MembershipAuditLogs.Update(audit);
    }

    public async Task Delete(MembershipAuditLog audit)
    {
        _context.MembershipAuditLogs.Remove(audit);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<MembershipAuditLog>> GetListAsync(DateTime? startDate, DateTime? endDate, long? staffId)
    {
        var query = _context.
            MembershipAuditLogs
            .Include(a => a.Staff)
            .Include(a => a.Member)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date < endDate.Value.Date.AddDays(1));
        }

        if (staffId.HasValue)
        {
            query = query.Where(a => a.StaffId == staffId.Value);
        }

        return await query.ToListAsync();
    }
}