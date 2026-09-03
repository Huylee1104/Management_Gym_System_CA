using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;

public class AuditLogService : IAuditLogService
{
    private readonly IMembershipAuditLogRepository _auditLogRepo;


    public AuditLogService(IMembershipAuditLogRepository auditLogRepo)
    {
        _auditLogRepo = auditLogRepo;
    }

    public async Task<ActivityLogResponse> GetActivityLogsAsync(ActivityLogRequest request)
    {
        var listLog = await _auditLogRepo.GetListAsync(request.FromDate, request.ToDate, request.StaffId);

        if (!string.IsNullOrWhiteSpace(request.Action))
            listLog = listLog.Where(x => x.Action == request.Action).ToList();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            listLog = listLog.Where(x => x.Member is { } member && member.FullName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        var totalItems = listLog.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var items = listLog.OrderByDescending(x => x.Date)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ActivityLogDto
            {
                Id = x.Id,
                Date = x.Date,
                StaffName = x.Staff?.FullName,
                Action = x.Action,
                MemberName = x.Member?.FullName,
                Note = x.Note
            }).ToList();

        return new ActivityLogResponse { 
            Items = items, 
            Page = request.Page, 
            PageSize = request.PageSize, 
            TotalItems = totalItems, 
            TotalPages = totalPages 
        };
    }
}