using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudge.Models;
using Nudge.Services;
using Nudge.Services.CarerService;
using SharecareAPI.Models;
using SharecareAPI.Services.PaitentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nudge.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly CarerService _carerservice;
        private readonly AdminService _adminService;
        private readonly PaitentService _patientService;


        public AdminController(CarerService userService, AdminService adminService, PaitentService patientService)
        {
            _carerservice = userService;
            _adminService = adminService;
            _patientService = patientService;
        }

        /// <summary>
        /// Creates a new Administrator
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Carer> CreateAdmin(NewUser user)
        {        
            try
            {
                return _adminService.CreateAdmin(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Auth token is incorrect for this user" });
            }

        }

        /// <summary>
        /// Creates a new Patient
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<String> CreatePatient(Patient patient)
        {
            try
            {
                return _patientService.CreatePatient(patient);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Auth token is incorrect for this user" });
            }

        }

        /// <summary>
        /// Gets All Active Nudge Users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Carer>> GetActiveUsers([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                return _carerservice.GetActiveUsers();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Auth token is incorrect for this user" });
            }
        }

        /// <summary>
        /// Gets User by username.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<Carer> GetUserByUsername(string username)
        {
            return _carerservice.GetUserByUsername(username);
        }

        /// <summary>
        /// Gets UserId by username.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetUserIdByUsername(string username) =>
            _carerservice.GetUserIdByUsername(username);


    }
}
