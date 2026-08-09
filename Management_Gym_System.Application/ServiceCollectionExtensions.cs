using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Management_Gym_System.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Tầng Infrastructure trực tiếp quản lý các Repositories
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<ICheckinService, CheckinService>();
        services.AddScoped<IGymMembershipCardService, GymMembershipCardService>();
        services.AddScoped<IInventoryImportService, InventoryImportService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRolesService, RolesService>();

        return services;
    }
}