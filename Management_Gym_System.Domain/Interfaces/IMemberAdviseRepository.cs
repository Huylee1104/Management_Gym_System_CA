using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Domain.Interfaces;

public interface IMemberAdviseRepository
{
    Task<List<MemberAdvise>> GetListAdviseAsync(DateTime? fromDate, DateTime? toDate);
    Task<List<MemberAdvise>> GetListAdviseContactedAsync(DateTime? fromDate, DateTime? toDate);
    Task<MemberAdvise?> GetByIdAsync(long id);
    Task AddAsync(MemberAdvise memberAdvise);
    Task UpdateAsync(MemberAdvise memberAdvise);
    Task<bool> SaveChangesAsync();
}