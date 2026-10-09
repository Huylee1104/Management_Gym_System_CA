using Management_Gym_System.Application.DTOs.MemberAdvise;
using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IMemberAdviseService
{
    Task<List<MemberAdvise>> GetListMemberAdviseAsync(DateTime? fromDate, DateTime? toDate, string? fullName);
    Task<List<MemberAdvise>> GetListMemberAdviseContactedAsync(DateTime? fromDate, DateTime? toDate, string? fullName);
    Task<ServiceResult> AddMemberAdviseAsync(RequestMemberAdvise requestMemberAdvise);
    Task<ServiceResult> UpdateMemberAdviseAsync(MemberAdvise memberAdvise);
    Task<ServiceResult> ReviewProductAsync(long ProductId, string Review);
}