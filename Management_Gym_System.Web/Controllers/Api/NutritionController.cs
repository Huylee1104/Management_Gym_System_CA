using Management_Gym_System.Application.Services;
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
        public NutritionController(INutritionGroupService nutritionGroupService,
            INutritionDetailService nutritionDetailService,
            CurrentUserHelper currentUserHelper)
        {
            _nutritionDetailService = nutritionDetailService;
            _nutritionGroupService = nutritionGroupService;
            _currentUserHelper = currentUserHelper;
        }

        public IActionResult Index()
        {
            ViewBag.IsAdmin = _currentUserHelper.IsAdmin();
            return View("~/Views/Nutrition/Index.cshtml");
        }


    }
}