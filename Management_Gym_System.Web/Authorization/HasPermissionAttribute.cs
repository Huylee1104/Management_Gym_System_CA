using System.Security.Claims;
using Management_Gym_System.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Management_Gym_System.Web.Authorization;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string actionCode)
        : base(typeof(HasPermissionFilter))
    {
        Arguments = new object[] { actionCode };
    }
}

public class HasPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _actionCode;
    private readonly IPermissionService _permissionService;

    public HasPermissionFilter(
        string actionCode,
        IPermissionService permissionService)
    {
        _actionCode = actionCode;
        _permissionService = permissionService;
    }

    public async Task OnAuthorizationAsync(
        AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity == null ||
            !user.Identity.IsAuthenticated)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var userIdClaim =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userIdClaim, out var userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasPermission =
            await _permissionService.HasPermissionAsync(
                userId,
                _actionCode);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}