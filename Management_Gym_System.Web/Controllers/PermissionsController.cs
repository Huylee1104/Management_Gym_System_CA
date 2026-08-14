using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Controllers;

public class PermissionController : Controller
{
    [HttpGet]
    [HasPermission("PERMISSION_VIEW")]
    public IActionResult Index()
    {
        return View("~/Views/Permissions/Index.cshtml");
    }

    [HttpGet]
    [HasPermission("PERMISSION_VIEW")]
    public IActionResult RolePermissions(long roleId)
    {
        ViewBag.RoleId = roleId;

        return View(
            "~/Views/Permissions/RolePermissions.cshtml");
    }
}