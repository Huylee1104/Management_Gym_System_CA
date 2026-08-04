using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class Checkin
{
    [Key]
    public long ID { get; set; }

    public long? CardID { get; set; }

    public DateTime? CheckinTime { get; set; }

    public string? Status { get; set; }

    // Navigation property
    [ForeignKey("CardID")]
    public GymMembershipCard? Card { get; set; } = null!;
}
