using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharecareAPI.Models;
using ShareCare.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using ShareCare.Services.CarerService;

namespace SharecareAPI.Services.PaitentServices
{
    public class PaitentService
    {
        private readonly IMongoCollection<Patient> _patients;

        public PaitentService(IShareCareDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            _patients = database.GetCollection<Patient>(settings.PatientCollection);
        }

        public Patient GetPatient(string patientId)
        {
            var x = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the patient
            return x ?? null;
        }

        public IEnumerable<Appointment> GetPatientAppointments(string patientId)
        {
            var patient = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the patient

            var upcomingAppointments = patient.Appointments != null ? patient.Appointments.Where(c => true).ToList() : null;

            if (upcomingAppointments != null)
            {
                return upcomingAppointments.ToList();
            }

            return Enumerable.Empty<Appointment>();
        }

        public String CreatePatient(Patient user)
        {
                var x = new Patient()
                {
                    FullName = user.FullName,
                    Record = user.Record,
                    Comments = null,
                    AdmissionDate = DateTime.Now,
                    Medicine = user.Medicine
                };

                _patients.InsertOne(x);
                return x.Id;
        }

        public bool CreateAppointment(string patientId, Appointment appointment)
        {
            var patient = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the patient

            if (patient != null)
            {
                //update patients appointments in the database
                _patients.UpdateOneAsync(
                    Builders<Patient>.Filter.Eq(x => x.Id, patient.Id),
                    Builders<Patient>.Update.AddToSet(x => x.Appointments, appointment));

                return true;
            }

            return true;
        }

        public bool CommentOnPatient(string carerId, string patientId, CarerComments comment)
        {
            var patient = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the patient

            if (patient != null)
            {
                //update patients comments in the database
                _patients.UpdateOneAsync(
                    Builders<Patient>.Filter.Eq(x => x.Id, patient.Id),
                    Builders<Patient>.Update.AddToSet(x => x.Comments, comment));

                return true;
            }

            return false;
        }
        
        public IEnumerable<Patient> SearchPatients(string patientName)
        {
            var patients = _patients.Find(c => c.FullName.ToLower().Contains(patientName.ToLower())).ToList();//get the patients that match search

            if (patients != null)
            {
                return patients;
            }

            return Enumerable.Empty<Patient>();
        }

        public List<Patient> GetAllPatients() =>
          _patients.Find(patieny => true).ToList();


    }
}
