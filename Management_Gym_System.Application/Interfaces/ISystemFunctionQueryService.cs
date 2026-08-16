

using Management_Gym_System.Application.DTOs.SystemFunction;

namespace Management_Gym_System.Application.Interfaces;

public interface ISystemFunctionQueryService
{
    Task<List<SystemFunctionDto>> GetFunctionsAsync();
}