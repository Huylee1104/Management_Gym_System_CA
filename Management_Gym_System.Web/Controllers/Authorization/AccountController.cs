using System.Security.Claims;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Web.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AccountController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Login / Logout / AccessDenied

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == model.Username && u.Status == true);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại hoặc đã bị khóa.");
            return View(model);
        }

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("FullName", user.FullName ?? string.Empty),
            new Claim("RoleId", user.RoleID?.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion

    #region Register

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isExist = await _dbContext.Users.AnyAsync(u => u.Username == model.Username);
        if (isExist)
        {
            ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng.");
            return View(model);
        }

        var passwordHasher = new PasswordHasher<User>();
        var newUser = new User
        {
            Username = model.Username,
            FullName = model.FullName,
            PhoneNumber = model.PhoneNumber,
            Status = true
        };
        newUser.PasswordHash = passwordHasher.HashPassword(newUser, model.Password);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
        return RedirectToAction("Login");
    }

    #endregion

    #region Profile & Change Password

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login");
        }

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.ID == userId);

        if (user == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            ID = user.ID,
            Username = user.Username,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.Role?.RoleName ?? "Chưa phân vai trò"
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login");
        }

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
        return RedirectToAction("Profile");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out var userId))
        {
            return RedirectToAction("Login");
        }

        if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin mật khẩu.";
            return RedirectToAction("Profile");
        }

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var passwordHasher = new PasswordHasher<User>();
        var verifyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác.";
            return RedirectToAction("Profile");
        }

        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("Profile");
    }

    #endregion
}