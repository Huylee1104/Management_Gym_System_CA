using Management_Gym_System.Domain.Entities;

public class MemberAdvise
{
    public long Id { get; set; }
    public long? ProductId { get; set; }
    public DateTime Date { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool? IsContacted { get; set; }
    public DateTime? ContactedDate { get; set; }
    public int ? ContactedCount { get; set; }
}