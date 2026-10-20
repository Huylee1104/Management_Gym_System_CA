using Management_Gym_System.Application.Services;
using Management_Gym_System.Application.Services.Cloudinary;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Web.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class NutritionController : Controller
    {
        private readonly INutritionGroupService _nutritionGroupService;
        private readonly INutritionDetailService _nutritionDetailService;
        private readonly CurrentUserHelper _currentUserHelper;
        private readonly CloudinaryService _cloudinaryService;
        public NutritionController(INutritionGroupService nutritionGroupService,
            INutritionDetailService nutritionDetailService,
            CurrentUserHelper currentUserHelper,
            CloudinaryService cloudinaryService)
        {
            _nutritionDetailService = nutritionDetailService;
            _nutritionGroupService = nutritionGroupService;
            _currentUserHelper = currentUserHelper;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            ViewBag.IsAdmin = _currentUserHelper.IsAdmin();
            return View("~/Views/Nutrition/Index.cshtml");
        }

        [HttpPost("GetGroups")]
        public async Task<IActionResult> GetGroups()
        {
            bool isAdmin = _currentUserHelper.IsAdmin();
            IReadOnlyList<NutritionGroup> groups = await _nutritionGroupService.GetAllGroupsAsync(isAdmin);

            var result = groups.Select(g => new
            {
                id = g.Id,
                name = g.Name,
                description = g.Description,
                imageUrl = g.ImageUrl,
                sortOrder = g.SortOrder,
                isActive = g.IsActive
            }).ToList();

            return Json(new { success = true, data = result });
        }

        [HttpPost("GetGroupById")]
        public async Task<IActionResult> GetGroupById([FromForm] long id)
        {
            NutritionGroup? group = await _nutritionGroupService.GetGroupByIdAsync(id);
            if (group == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhóm thực phẩm." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    id = group.Id,
                    name = group.Name,
                    description = group.Description,
                    imageUrl = group.ImageUrl,
                    sortOrder = group.SortOrder,
                    isActive = group.IsActive
                }
            });
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup([FromForm] NutritionGroup model, IFormFile? imageFile)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, message = "Tên nhóm thực phẩm không được để trống." });
            }

            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                await using var stream = imageFile.OpenReadStream();
                imageUrl = await _cloudinaryService.UploadImageAsync(stream: stream,
                    originalFileName: imageFile.FileName, folder: "nutrition-groups");
            }

            model.ImageUrl = imageUrl;

            await _nutritionGroupService.CreateGroupAsync(model);
            return Json(new { success = true, message = "Thêm nhóm thực phẩm thành công." });
        }

        [HttpPost("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup([FromForm] NutritionGroup model, IFormFile? imageFile)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, message = "Tên nhóm thực phẩm không được để trống." });
            }

            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                await using var stream = imageFile.OpenReadStream();
                imageUrl = await _cloudinaryService.UploadImageAsync(stream: stream,
                    originalFileName: imageFile.FileName, folder: "nutrition-groups");
            }

            model.ImageUrl = imageUrl;

            bool isUpdated = await _nutritionGroupService.UpdateGroupAsync(model);
            if (isUpdated == false)
            {
                return Json(new { success = false, message = "Không tìm thấy nhóm thực phẩm để cập nhật." });
            }

            return Json(new { success = true, message = "Cập nhật nhóm thực phẩm thành công." });
        }

        [HttpPost("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup([FromForm] long id)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            bool isDeleted = await _nutritionGroupService.DeleteGroupAsync(id);
            if (isDeleted == false)
            {
                return Json(new { success = false, message = "Không tìm thấy nhóm thực phẩm để xóa." });
            }

            return Json(new { success = true, message = "Xóa nhóm thực phẩm thành công." });
        }

        // ==========================================
        // API: THỰC PHẨM CHI TIẾT (NutritionDetail)
        // ==========================================

        [HttpPost("GetDetailsByGroup")]
        public async Task<IActionResult> GetDetailsByGroup([FromForm] long groupId)
        {
            bool isAdmin = _currentUserHelper.IsAdmin();
            List<NutritionDetail> details = await _nutritionDetailService.GetDetailsByGroupIdAsync(groupId, isAdmin);

            var result = details.Select(d => new
            {
                id = d.Id,
                nutritionGroupId = d.NutritionGroupId,
                name = d.Name,
                calories = d.Calories,
                servingSize = d.ServingSize,
                servingUnit = d.ServingUnit,
                isPublished = d.IsPublished
            }).ToList();

            return Json(new { success = true, data = result });
        }

        [HttpPost("GetDetailById")]
        public async Task<IActionResult> GetDetailById([FromForm] long id)
        {
            NutritionDetail? detail = await _nutritionDetailService.GetDetailByIdAsync(id);
            if (detail == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin thực phẩm." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    id = detail.Id,
                    nutritionGroupId = detail.NutritionGroupId,
                    groupName = detail.NutritionGroup != null ? detail.NutritionGroup.Name : "",
                    name = detail.Name,
                    description = detail.Description,
                    imageUrl = detail.ImageUrl,
                    calories = detail.Calories,
                    protein = detail.Protein,
                    carb = detail.Carb,
                    fat = detail.Fat,
                    fiber = detail.Fiber,
                    sugar = detail.Sugar,
                    sodium = detail.Sodium,
                    servingSize = detail.ServingSize,
                    servingUnit = detail.ServingUnit,
                    recommendedTime = detail.RecommendedTime,
                    recommendedTarget = detail.RecommendedTarget,
                    pros = detail.Pros,
                    cons = detail.Cons,
                    isPublished = detail.IsPublished
                }
            });
        }

        [HttpPost("CreateDetail")]
        public async Task<IActionResult> CreateDetail([FromForm] NutritionDetail model, IFormFile? imageFile)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (model.NutritionGroupId <= 0)
            {
                return Json(new { success = false, message = "Thực phẩm bắt buộc phải thuộc về một nhóm." });
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, message = "Tên thực phẩm không được để trống." });
            }

            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                await using var stream = imageFile.OpenReadStream();
                imageUrl = await _cloudinaryService.UploadImageAsync(stream: stream,
                    originalFileName: imageFile.FileName, folder: "nutrition-details");
            }

            model.ImageUrl = imageUrl;

            await _nutritionDetailService.CreateDetailAsync(model);
            return Json(new { success = true, message = "Thêm thực phẩm thành công." });
        }

        [HttpPost("UpdateDetail")]
        public async Task<IActionResult> UpdateDetail([FromForm] NutritionDetail model, IFormFile? imageFile)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            if (model.NutritionGroupId <= 0)
            {
                return Json(new { success = false, message = "Thực phẩm bắt buộc phải thuộc về một nhóm." });
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, message = "Tên thực phẩm không được để trống." });
            }

            string? imageUrl = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                await using var stream = imageFile.OpenReadStream();
                imageUrl = await _cloudinaryService.UploadImageAsync(stream: stream,
                    originalFileName: imageFile.FileName, folder: "nutrition-details");
            }

            model.ImageUrl = imageUrl;

            bool isUpdated = await _nutritionDetailService.UpdateDetailAsync(model);
            if (isUpdated == false)
            {
                return Json(new { success = false, message = "Không tìm thấy thực phẩm để cập nhật." });
            }

            return Json(new { success = true, message = "Cập nhật thực phẩm thành công." });
        }

        [HttpPost("DeleteDetail")]
        public async Task<IActionResult> DeleteDetail([FromForm] long id)
        {
            if (_currentUserHelper.IsAdmin() == false)
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            bool isDeleted = await _nutritionDetailService.DeleteDetailAsync(id);
            if (isDeleted == false)
            {
                return Json(new { success = false, message = "Không tìm thấy thực phẩm để xóa." });
            }

            return Json(new { success = true, message = "Xóa thực phẩm thành công." });
        }
    }
}