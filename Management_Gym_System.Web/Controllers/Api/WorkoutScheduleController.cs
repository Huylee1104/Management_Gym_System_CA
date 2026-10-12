using Management_Gym_System.Application.Interfaces;
using Management_Gym_System.Application.Services;
using Management_Gym_System.Domain.Entities;
using Management_Gym_System.Infrastructure.Data;
using Management_Gym_System.Services;
using Management_Gym_System.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management_Gym_System.Controllers.Api
{
    [Route("[Controller]")]
    [ApiController]
    public class WorkoutScheduleController : Controller
    {
        private readonly IGymMembershipCardService _gymMemberCard;
        public WorkoutScheduleController(IGymMembershipCardService gymMemberCard)
        {
            _gymMemberCard = gymMemberCard;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var account = await _gymMemberCard.GetInfoAcountMember();
            ViewBag.AcountMember = account;
            return View("~/Views/Home/HomeMember.cshtml");
        }

        [HttpPost("GetCalendar")]
        public async Task<IActionResult> GetCalendar()
        {
            var calender = await _gymMemberCard.getCalunder();
            return Ok(calender);
        }
    }
}