using Management_Gym_System.Application.DTOs.Permission;
using Management_Gym_System.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Controllers.Api;

    [Route("[Controller]")]
    [ApiController]
public class PermissionController : Controller
{
    private readonly IPermissionService _permissionService;

    public PermissionController(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    // [HttpGet]
    // public IActionResult Index()
    // {
    //     return View("~/Views/Permissions/Index.cshtml");
    // }

    [HttpGet("functions")]
    public async Task<IActionResult> GetFunctions()
    {
        var result =
            await _permissionService.GetFunctionsAsync();

        return Ok(result);
    }

    [HttpPost("functions")]
    public async Task<IActionResult> CreateFunction(
        [FromBody] SystemFunction function)
    {
        if (function == null ||
            string.IsNullOrWhiteSpace(function.Code) ||
            string.IsNullOrWhiteSpace(function.Name))
        {
            return BadRequest("Dữ liệu không hợp lệ.");
        }

        var id =
            await _permissionService.CreateFunctionAsync(function);

        return Ok(new
        {
            success = true,
            id
        });
    }

    [HttpPut("functions/{id}")]
    public async Task<IActionResult> UpdateFunction(
        long id,
        [FromBody] SystemFunction function)
    {
        var success =
            await _permissionService.UpdateFunctionAsync(
                id,
                function);

        if (!success)
            return NotFound();

        return Ok(new
        {
            success = true
        });
    }

    [HttpPost("actions")]
    public async Task<IActionResult> CreateAction(
        [FromBody] SystemFunctionAction action)
    {
        if (action == null ||
            action.FunctionId <= 0 ||
            string.IsNullOrWhiteSpace(action.Code) ||
            string.IsNullOrWhiteSpace(action.ActionName) ||
            string.IsNullOrWhiteSpace(action.DisplayName))
        {
            return BadRequest("Dữ liệu Action không hợp lệ.");
        }

        var id =
            await _permissionService.CreateActionAsync(action);

        return Ok(new
        {
            success = true,
            id
        });
    }

    [HttpPut("actions/{id}")]
    public async Task<IActionResult> UpdateAction(
        long id,
        [FromBody] SystemFunctionAction action)
    {
        var success =
            await _permissionService.UpdateActionAsync(
                id,
                action);

        if (!success)
            return NotFound();

        return Ok(new
        {
            success = true
        });
    }

    [HttpGet("roles/{roleId}")]
    public async Task<IActionResult> GetRolePermissions(
        long roleId)
    {
        var result =
            await _permissionService
                .GetPermissionTreeAsync(roleId);

        return Ok(result);
    }

    [HttpPost("roles/{roleId}")]
    public async Task<IActionResult> SaveRolePermissions(
        long roleId,
        [FromBody] List<PermissionItemRequest> permissions)
    {
        await _permissionService
            .SaveRolePermissionsAsync(
                roleId,
                permissions);

        return Ok(new
        {
            success = true,
            message = "Lưu phân quyền thành công."
        });
    }

    // [HttpGet("roles/{roleId}/page")]
    // public async Task<IActionResult> RolePermissionsPage(long roleId)
    // {
    //     ViewBag.RoleId = roleId;

    //     return View(
    //         "~/Views/Permissions/RolePermissions.cshtml");
    // }
}