using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("listProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] long? categoryId, [FromQuery] string? keyword)
        {
            var result = await _productService.GetProductsAsync(categoryId, keyword);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var result = await _productService.CreateAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                var success = await _productService.UpdateAsync(id, request);
                if (!success) return NotFound();
                return Ok(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var success = await _productService.ToggleStatusAsync(id);
            if (!success) return NotFound();
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success) return NotFound(new { success = false, message = "Không tìm thấy sản phẩm!" });
            return Ok(new { success = true });
        }
    }
}