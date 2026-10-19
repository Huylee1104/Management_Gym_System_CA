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
        public NutritionController()
        {
        }

        public IActionResult Index()
        {
            ViewBag.IsAdmin = true;
            return View("~/Views/Nutrition/Index.cshtml");
        }
    }
}