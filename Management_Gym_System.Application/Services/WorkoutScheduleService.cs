using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Interfaces;

public class WorkoutScheduleService : IWorkoutScheduleService
{
    private readonly CurrentUserHelper _currentUser;
    private readonly IWorkoutScheduleRepository _workoutScheduleRepository;
    public WorkoutScheduleService(CurrentUserHelper currentUser, IWorkoutScheduleRepository workoutScheduleRepository)
    {
        _currentUser = currentUser;
        _workoutScheduleRepository = workoutScheduleRepository;
    }
}