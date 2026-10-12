namespace Management_Gym_System.Domain.Entities;

public class WorkoutNoteDetail
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string? MemberNote { get; set; }
    public DateTime? MemberNoteDate { get; set; }
    public string? TrainerNote { get; set; }
    public DateTime? TrainerNoteDate { get; set; }
    public virtual User? User { get; set; }
    //public long? WorkoutScheduleId { get; set; }
    //public virtual WorkoutSchedule? WorkoutSchedule { get; set; }

}