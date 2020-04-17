using System;
using SharecareAPI.Models;
using SharecareAPI.Services.PaitentServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareCare.Models;
using Microsoft.AspNetCore.Authorization;
using ShareCare.Services.CarerService;
using System.Security.Claims;

namespace ShareCare.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarerController : ControllerBase
    {   
        private readonly CarerService _carerservice;
        private readonly PaitentService _patientService;

        public CarerController(CarerService userService, PaitentService paitentService)
        {
            _carerservice = userService;
            _patientService = paitentService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var token = _carerservice.Authenticate(model.Username, model.Password);

            if (token == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(token);
        }

        /// <summary>
        /// Get current logged in user.
        /// </summary>
        /// <returns>User Object</returns>
        /// 
        [HttpGet]
        public ActionResult<Carer> GetLoggedInUser()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                return _carerservice.GetActiveUser(userIdClaim);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e });
            }
        }

        [HttpGet]
        public object Claims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }

        /// <summary>
        /// Creates a new Carer.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<string> Register(NewUser user)
        {
            var x = _carerservice.CreateUser(user);
            return x.Id;
        }

        /// <summary>
        /// Comment on Patient.
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<bool> CommentOnPatient(string patientId, CarerComments comment)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var carerNameClaim = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var x = _patientService.CommentOnPatient(carerNameClaim, patientId, comment);

            return x;
        }

        /// <summary>
        /// Get Work dates for Carer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<WorkingDay>> GetRota()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var workingDays = _carerservice.GetMyRota(userIdClaim).ToArray();

            if (workingDays == Enumerable.Empty<WorkingDay>())
            {
                return NotFound();
            }

            return workingDays;
        }

    }
}
