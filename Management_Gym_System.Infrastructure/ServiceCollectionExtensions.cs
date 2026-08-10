using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Infrastructure.Queries;
using Management_Gym_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Management_Gym_System.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Đăng ký DbContext và chỉ định vị trí Migrations
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            ));
            
        // Tầng Infrastructure trực tiếp quản lý các Repositories
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<ICheckinRepository, CheckinRepository>();
        services.AddScoped<IGymMembershipCardRepository, GymMembershipCardRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IInventoryImportRepository, InventoryImportRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        services.AddScoped<IInventoryImportQueryService, InventoryImportQueryService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}