using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharecareAPI.Models;
using Nudge.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Nudge.Services.CarerService;

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
            var x = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the user
            return x ?? null;
        }

        public List<Appointments> GetPatientAppointments(string patientId)
        {
            var x = _patients.Find(c => c.Id == patientId).FirstOrDefault();//get the user
            return x.Appointments.ToList() ?? new List<Appointments>().Concat(x.Appointments.ToList() ?? new List<Appointments>()).ToList();
        }

        public String CreatePatient(Patient user)
        {
                var x = new Patient()
                {
                    FullName = user.FullName,
                    Record = user.Record,
                    Comments = null,
                    AdmissionDate = DateTime.Now,
                    Appointments = null
                };

                _patients.InsertOne(x);
                return x.Id;
        }

    }
}
