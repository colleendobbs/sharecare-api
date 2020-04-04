using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShareCare.Models
{
    public class Role
    {
        [JsonIgnore]
        public string RoleName { get; set; }
    }
}
