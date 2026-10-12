using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class MemberAdviseController : Controller
    {
        public MemberAdviseController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View("~/Views/MemberAdvise/Index.cshtml");
        }

        
    }
}