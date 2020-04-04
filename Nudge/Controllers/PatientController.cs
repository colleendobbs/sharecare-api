using SharecareAPI.Models;
using SharecareAPI.Services.PaitentServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.ComponentModel.DataAnnotations;

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
        /// Gets a patient.
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
        /// Gets a patients upcoming appointments.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Appointment>> GetPatientAppointments(string patientId)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var AppointMents = _patientService.GetPatientAppointments(patientId).ToArray();

            if (AppointMents == null)
            {
                return NotFound();
            }

            return AppointMents;
        }


        /// <summary>
        /// Returns patient that match inputted search query.
        /// </summary>
        /// <param name="patientName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> SearchPatientByName([Required]string patientName)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var results = _patientService.SearchPatients(patientName).ToArray();

            if (results == null)
            {
                return NotFound();
            }

            return results;
        }



    }
}