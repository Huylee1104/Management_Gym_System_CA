using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Web.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class RegisterMembershipController : Controller
    {
        private readonly IProductService _productService;

        public RegisterMembershipController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/RegisterMembership/Index.cshtml");
        }

        [HttpPost("GetProducts")]
        public async Task<IActionResult> GetProducts()  
        {
            var products = await _productService.GetProductsAsync(0, null);
            var goiTap = products.Where(p => p.ThoiHan != null).ToList();
            return Ok(goiTap);
        }
    }
}