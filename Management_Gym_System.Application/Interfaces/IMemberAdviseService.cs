using Management_Gym_System.Application.DTOs.MemberAdvise;
using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IMemberAdviseService
{
    Task<List<AdviseCustomerDto>> GetListMemberAdviseAsync(AdviseFilterRequest filter);
    Task<ServiceResult> AddMemberAdviseAsync(RequestMemberAdvise requestMemberAdvise);
    Task<ServiceResult> UpdateMemberAdviseAsync(MemberAdvise memberAdvise);
    Task<ServiceResult> ReviewProductAsync(long ProductId, string Review);
    Task<byte[]> ExportPdfAsync(AdviseFilterRequest filter);
    Task<byte[]> ExportExcelAsync(AdviseFilterRequest filter);
}