using Management_Gym_System.Application.DTOs.Inventory;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Dapper;
using Management_Gym_System.Application.DTOs.SystemFunction;

namespace Management_Gym_System.Infrastructure.Queries;

public class SystemFunctionQueryService : ISystemFunctionQueryService
{
    private readonly ApplicationDbContext _context;

    public SystemFunctionQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemFunctionDto>> GetFunctionsAsync()
    {
        return await _context.SystemFunctions
            .Include(x => x.Actions)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new SystemFunctionDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Controller = x.Controller,
                Description = x.Description,
                IsActive = x.IsActive,
                DisplayOrder = x.DisplayOrder,

                Actions = x.Actions
                    .OrderBy(a => a.DisplayOrder)
                    .Select(a => new SystemFunctionActionDto
                    {
                        Id = a.Id,
                        FunctionId = a.FunctionId,
                        Code = a.Code,
                        ActionName = a.ActionName,
                        DisplayName = a.DisplayName,
                        Description = a.Description,
                        IsActive = a.IsActive,
                        DisplayOrder = a.DisplayOrder
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}