using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Web.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _categoryService;

        // Controller CHI INJECT 1 SERVICE DUY NHẤT! Không còn DbContext hay GenericService nữa!
        public ProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Categories/Index.cshtml");
        }

        [HttpGet("listCategories")]
        public async Task<IActionResult> GetCategories(string? keyword)
        {
            var categories = await _categoryService.GetCategoriesAsync(keyword);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ProductCategory category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _categoryService.CreateCategoryAsync(category);
            return Ok(category);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCategory(long id, [FromBody] ProductCategory category)
        {
            if (id != category.ID) return BadRequest();

            var result = await _categoryService.UpdateCategoryAsync(id, category);
            if (!result) return NotFound();

            return Ok(category);
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _categoryService.ToggleStatusAsync(id);
            if (!result) return NotFound();

            return Ok(new { success = true });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(long id) // Đã sửa kiểu int -> long cho đồng nhất
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return Json(new { success = false, message = "Không tìm thấy danh mục!" });

            return Json(new { success = true });
        }
    }
}