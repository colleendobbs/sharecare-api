using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SharecareAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nudge.Models
{
    public class Carer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [BsonElement("firstname")]
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        
        [BsonElement("lastName")]
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [BsonElement("email")]
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [JsonIgnore]
        [BsonElement("password")]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [JsonIgnore]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [BsonElement("roles")]
        public string Role { get; set; }

        public string Token { get; set; }

        [BsonIgnoreIfNull]//this is so the object is not initialized until the user actually has a request,
        [BsonElement("rota")]
        public DateTime[] WorkingDays { get; set; }

    }
}
