using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Controllers;

public class PermissionsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Permissions/Index.cshtml");
    }

    [HttpGet]
    public IActionResult RolePermissions(long roleId)
    {
        ViewBag.RoleId = roleId;

        return View(
            "~/Views/Permissions/RolePermissions.cshtml");
    }
}