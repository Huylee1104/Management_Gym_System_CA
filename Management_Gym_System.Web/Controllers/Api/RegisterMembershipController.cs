using System.Security.Claims;
using Management_Gym_System.Application.DTOs.MemberAdvise;
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
        private readonly ILogger<RegisterMembershipController> _logger;
        private readonly IMemberAdviseService _memberAdviseService;
        private readonly IUsersService _userService;

        public RegisterMembershipController(IProductService productService,
            ILogger<RegisterMembershipController> logger, IMemberAdviseService memberAdviseService,
            IUsersService userService)
        {
            _productService = productService;
            _logger = logger;
            _memberAdviseService = memberAdviseService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.IsAuthenticated = User.FindFirst(ClaimTypes.NameIdentifier) != null;
            return View("~/Views/RegisterMembership/Index.cshtml");
        }

        [HttpPost("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync(0, null);
            var goiTap = products.Where(p => p.ThoiHan != null).ToList();
            return Ok(goiTap);
        }

        [HttpPost("GetMemberAdvises")]
        public async Task<IActionResult> GetMemberAdvises(DateTime? fromDate, DateTime? toDate, string? fullName)
        {
            var memberAdvises = await _memberAdviseService.GetListMemberAdviseAsync(fromDate, toDate, fullName);
            return Ok(memberAdvises);
        }

        [HttpPost("GetMemberAdvisesContacted")]
        public async Task<IActionResult> GetMemberAdvisesContacted(DateTime? fromDate, DateTime? toDate, string? fullName)
        {
            var memberAdvises = await _memberAdviseService.GetListMemberAdviseContactedAsync(fromDate, toDate, fullName);
            return Ok(memberAdvises);
        }

        [HttpPost("AddMemberAdvise")]
        public async Task<IActionResult> AddMemberAdvise([FromBody] RequestMemberAdvise requestMemberAdvise)
        {
            var result = await _memberAdviseService.AddMemberAdviseAsync(requestMemberAdvise);
            return Ok(result);
        }

        [HttpPost("UpdateMemberAdvise")]
        public async Task<IActionResult> UpdateMemberAdvise(MemberAdvise memberAdvise)
        {
            var result = await _memberAdviseService.UpdateMemberAdviseAsync(memberAdvise);
            return Ok(result);
        }

        [HttpPost("ReviewProduct")]
        public async Task<IActionResult> ReviewProduct(long ProductId, string Review)
        {
            var result = await _memberAdviseService.ReviewProductAsync(ProductId, Review);
            return Ok(result);
        }
    }
}