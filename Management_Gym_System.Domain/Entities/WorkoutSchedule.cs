namespace Management_Gym_System.Domain.Entities;

public class WorkoutSchedule
{
    public long Id { get; set; }
    public long? CardId { get; set; }
    public long? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PauseDate { get; set; }
    public DateTime? ResumeDate { get; set; }
    public DateTime? CheckinTime { get; set; }

    public virtual GymMembershipCard? Card { get; set; }
    public virtual User? User { get; set; }

    public virtual ICollection<WorkoutNoteDetail> WorkoutNoteDetails { get; set; }
        = new List<WorkoutNoteDetail>();
}