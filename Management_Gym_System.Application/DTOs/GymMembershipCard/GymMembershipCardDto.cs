public class GymMembershipCardDto
{
    public long ID { get; set; }
    public string? RFID_UID { get; set; } = string.Empty;
    public bool? Status { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? PauseDate { get; set; }
    public string? ResumeDate { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public string? ProductName { get; set; } = string.Empty;

}