using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharecareAPI.Models
{
    public class SharecareDatabaseSettings : IShareCareDatabaseSettings
    {
        public string PatientCollection { get; set; }
        public string CarerCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IShareCareDatabaseSettings
    {
        string PatientCollection { get; set; }
        string CarerCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}