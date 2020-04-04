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
        /// Creates a new Patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="appointment"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateNewPatientAppointment(string patientId, Appointment appointment)
        {
            try
            {
                 _patientService.CreateAppointment(patientId, appointment);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Auth token is incorrect for this user" });
            }

            return NoContent();
        }

        /// <summary>
        /// Update Carers Rota
        /// </summary>
        /// <param name="carerId"></param>
        /// <param name="nextWorkingDays"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCarersRota(string carerId, DateTime[] nextWorkingDays)
        {
            try
            {
                _carerservice.AddDaysToRota(carerId, nextWorkingDays);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Auth token is incorrect for this user" });
            }

            return NoContent();
        }

        /// <summary>
        /// Gets All Active Carers in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Carer>> GetActiveCarers([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                return _carerservice.GetAllCarers();
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
        public ActionResult<Carer> GetCarerByUsername(string username)
        {
            return _carerservice.GetCarerByUsername(username);
        }

        /// <summary>
        /// Gets UserId by username.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> GetCarerIdByUsername(string username) =>
            _carerservice.GetCarerIdByUsername(username);


    }
}
