using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Domain.Interfaces;
using Management_Gym_System.Infrastructure.Repositories; // Hoặc namespace chứa ProductCategoryRepository của bạn
using Management_Gym_System.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Thiết lập địa chỉ/port Kestrel lắng nghe (khi chạy sau reverse proxy như Nginx)
//builder.WebHost.UseUrls("http://0.0.0.0:8090");

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Cấu hình DbContext sử dụng PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký Generic Service cho Dependency Injection
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();