using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nudge.Helpers;
using Nudge.Models;
using Nudge.Services.CarerService;
using SharecareAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nudge.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<Carer> _carers;
        private readonly AppSettings _appSettings;
        private readonly CarerService.CarerService _carerservice;

        public AdminService(IShareCareDatabaseSettings settings, IOptions<AppSettings> appSettings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);

            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _carers = database.GetCollection<Carer>(settings.CarerCollection);
            _appSettings = appSettings.Value;
            _carerservice = new CarerService.CarerService();
        }

        public Carer CreateAdmin(NewUser user)
        {
            if (_carerservice.ValidateNewUser(user) != null)
            {
                var x = new Carer()
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    Role = "Admin"
                };
                _carers.InsertOne(x);
                return x;
            }
            return null;
        }

    }
}
