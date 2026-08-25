using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;

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
}