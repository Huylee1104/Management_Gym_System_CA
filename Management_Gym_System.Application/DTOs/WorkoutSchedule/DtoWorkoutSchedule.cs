public class DtoWorkoutSchedule
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PauseDate { get; set; }
    public DateTime? ResumeDate { get; set; }
    public List<DateTime?> CheckinTime { get; set; } = new();
    public List<DtoWorkoutNoteDetail> WorkoutNoteDetails { get; set; } = new List<DtoWorkoutNoteDetail>();
}

public class DtoWorkoutNoteDetail
{
    public string? MemberNote { get; set; }
    public DateTime? MemberNoteDate { get; set; }
    public string? TrainerNote { get; set; }
    public DateTime? TrainerNoteDate { get; set; }

}

public class DtoInfoAcountMember
{
    public string? FullName {get; set;}
    public string? Packback {get; set;}
    public string? Trainer {get; set;}
    public DateTime? StartDate {get; set;}
    public DateTime? EndDate {get; set;}
    public string? Notify {get; set;}
}