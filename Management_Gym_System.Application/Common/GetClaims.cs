using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class CurrentUserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long GetIdAccountLogin()
    {
        var value = _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        }

        return userId;
    }

    public bool IsAdmin()
    {
        var userType = _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue("UserType");

        var roleId = _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue("RoleId");

        return string.IsNullOrEmpty(userType) &&
               !string.IsNullOrEmpty(roleId);
    }
}