using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class WorkoutScheduleRepository : IWorkoutScheduleRepository
{
    private readonly ApplicationDbContext _context;
    public WorkoutScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}