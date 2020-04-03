using SharecareAPI.Models;
using SharecareAPI.Services.PaitentServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SharecareAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly PaitentService _patientService;

        public PatientController(PaitentService PatientService)
        {
            _patientService = PatientService;
        }


        /// <summary>
        /// Gets current users active nudges aggregated.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<Patient> GetPatient(string patientId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var patient = _patientService.GetPatient(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        /// <summary>
        /// Gets current users active nudges aggregated.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Appointments>> GetPatientAppointments(string patientId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var AppointMents = _patientService.GetPatientAppointments(patientId);

            if (AppointMents == null)
            {
                return NotFound();
            }

            return AppointMents;
        }



    }
}