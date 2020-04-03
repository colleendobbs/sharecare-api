using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharecareAPI.Models;
using Newtonsoft.Json;
using Nudge.Models;

namespace SharecareAPI.Models
{
    public enum Moods
    {
        Happy,
        Sad,
        Frustrated,
        Confused
    }

    public class Patient
    {
        [JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string FullName { get; set; }

        [BsonElement("record")]
        public MedicalRecord Record { get; set; }        
        
        [BsonElement("admissionDate")]
        public DateTime AdmissionDate { get; set; }

        [BsonElement("comments")]
        public CarerComments[] Comments { get; set; }

        [BsonElement("appointments")]
        public Appointments[] Appointments { get; set; }

    }

    public class Appointments
    {
        public DateTime AppointmentDate { get; set; }
        public String Subject { get; set; }
        public String Doctor { get; set; }
    }

    public class MedicalRecord
    {
        public String BloodType { get; set; }
        public String[] PastIllnesess { get; set; } 
        public DateTime DOB { get; set; }
        public String[] Allergies { get; set; }
    }

    public class CarerComments
    {
        public Moods CurrentMood { get; set; }
        public String CurentCarerID { get; set; }
        public String Comments { get; set; }
    }
}       