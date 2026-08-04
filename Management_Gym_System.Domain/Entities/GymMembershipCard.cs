using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management_Gym_System.Domain.Entities;

public class GymMembershipCard
{
    [Key]
    public long ID { get; set; }

    [StringLength(100)]
    public string? RFID_UID { get; set; }
    public long? UserID { get; set; }
    public long? ProductID { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PauseDate { get; set; }
    public DateTime? ResumeDate { get; set; }
    public bool? Status { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public User? User { get; set; } = null!;

    [ForeignKey("ProductID")]
    public Product? Product { get; set; } = null!;

    public ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
}
