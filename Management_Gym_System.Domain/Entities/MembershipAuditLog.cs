using Management_Gym_System.Domain.Entities;

public class MembershipAuditLog
{
    public long Id { get; set; }

    public DateTime Date { get; set; }

    public long? StaffId { get; set; }

    public string Action { get; set; } = string.Empty;

    public long? MemberId { get; set; }

    public string? Note { get; set; }
    public string? DataEdited { get; set; }
    public User Staff { get; set; } = null!;
    public User Member { get; set; } = null!;
}