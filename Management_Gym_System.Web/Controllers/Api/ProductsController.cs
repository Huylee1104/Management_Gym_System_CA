using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IGenericService<Product> _productService;
        private readonly ApplicationDbContext _context;

        public ProductsController(IGenericService<Product> productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Products/Index.cshtml");
        }

        [HttpGet("listProducts")]
        public async Task<IActionResult> GetProducts(long? categoryId, string? keyword)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Lọc category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            // Tìm theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = StringHelper.NormalizeText(keyword);
                query = query.Where(p =>
                    EF.Functions.ILike(
                        EF.Functions.Unaccent(p.ProductName),
                        $"%{normalizedKeyword}%"));
            }

            var products = await query
                .Select(p => new
                {
                    p.ID,
                    p.ProductName,
                    p.Price,
                    p.Unit,
                    CategoryName = p.Category.CategoryName,
                    p.CategoryID,
                    p.ThoiHan,
                    p.Status,
                    p.ImageProduct,
                })
                .OrderBy(p => p.ID)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product.Price <= 0) return BadRequest("Price must be greater than zero.");
            
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            await _productService.AddAsync(product);
            return Ok(product);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateProduct(long id, [FromBody] Product product)
        {
            if (id != product.ID) return BadRequest();
            if (product.Price <= 0) return BadRequest("Price must be greater than zero.");

            var existing = await _productService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.ProductName = product.ProductName;
            existing.CategoryID = product.CategoryID;
            existing.Price = product.Price;
            existing.Unit = product.Unit;
            existing.ThoiHan = product.ThoiHan;
            existing.Status = product.Status;

            await _productService.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var existing = await _productService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Status = !existing.Status;
            await _productService.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}