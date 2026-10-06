using System.Diagnostics;
using Management_Gym_System.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Management_Gym_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("~/Views/Home/HomeStrange.cshtml");
        }

        public IActionResult IndexAdmin()
        {
            return View("~/Views/Home/HomeManager.cshtml");
        }

        public IActionResult IndexMember()
        {
            return View("~/Views/Home/HomeMember.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
