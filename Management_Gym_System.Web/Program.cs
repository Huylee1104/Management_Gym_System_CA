using Management_Gym_System.Services;
using Microsoft.EntityFrameworkCore;
using Management_Gym_System.Infrastructure.Data;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using Management_Gym_System.Application;
using Management_Gym_System.Infrastructure;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Thiết lập địa chỉ/port Kestrel lắng nghe (khi chạy sau reverse proxy như Nginx)
//builder.WebHost.UseUrls("http://0.0.0.0:8090");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký Generic Service cho Dependency Injection
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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