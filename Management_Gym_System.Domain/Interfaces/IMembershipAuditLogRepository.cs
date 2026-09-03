using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IMembershipAuditLogRepository
{
    Task AddAsync(MembershipAuditLog audit);
    Task Update(MembershipAuditLog audit);
    Task Delete(MembershipAuditLog audit);
    Task<bool> SaveChangesAsync();
    Task<IEnumerable<MembershipAuditLog>> GetListAsync(DateTime? startDate, DateTime? endDate, long? staffId);
}