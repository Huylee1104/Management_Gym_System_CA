using Management_Gym_System.Domain.Entities;

namespace Management_Gym_System.Application.Services;

public interface IAuditLogService
{
    Task<ActivityLogResponse> GetActivityLogsAsync(ActivityLogRequest request);
}