using System.Security.Claims;
using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Management_Gym_System.Web.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string functionCode, PermissionType permissionType) 
        : base(typeof(HasPermissionFilter))
    {
        Arguments = new object[] { functionCode, permissionType };
    }
}

public class HasPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _functionCode;
    private readonly PermissionType _permissionType;
    private readonly IPermissionService _permissionService;

    public HasPermissionFilter(string functionCode, PermissionType permissionType, IPermissionService permissionService)
    {
        _functionCode = functionCode;
        _permissionType = permissionType;
        _permissionService = permissionService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new ChallengeResult(); // Chuyển về trang Login nếu chưa xác thực
            return;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userIdClaim, out var userId))
        {
            context.Result = new ForbidResult(); // 403 Forbidden
            return;
        }

        var hasPermission = await _permissionService.HasPermissionAsync(userId, _functionCode, _permissionType);
        if (!hasPermission)
        {
            context.Result = new ForbidResult(); // 403 Forbidden nếu không đủ quyền
        }
    }
}