using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Queries;
using Management_Gym_System.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Management_Gym_System.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Tầng Infrastructure trực tiếp quản lý các Repositories
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<ICheckinRepository, CheckinRepository>();
        services.AddScoped<IGymMembershipCardRepository, GymMembershipCardRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IInventoryImportRepository, InventoryImportRepository>();

        services.AddScoped<IInventoryImportQueryService, InventoryImportQueryService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}