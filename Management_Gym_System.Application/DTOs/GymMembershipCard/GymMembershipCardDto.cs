public class GymMembershipCardDto
{
    public long ID { get; set; }
    public string? RFID_UID { get; set; } = string.Empty;
    public bool? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PauseDate { get; set; }
    public DateTime? ResumeDate { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string? ProductName { get; set; } = string.Empty;

}