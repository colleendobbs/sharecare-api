using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Nudge.Helpers;
using Nudge.Models;
using SharecareAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;


namespace Nudge.Services.CarerService
{
    public interface IUserService
    {
        string Authenticate(string username, string password);
    }

    public class CarerService : IUserService
    {
        private readonly IMongoCollection<Carer> _carers;
        private readonly AppSettings _appSettings;


        public CarerService(IShareCareDatabaseSettings settings, IOptions<AppSettings> appSettings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _carers = database.GetCollection<Carer>(settings.CarerCollection);
            _appSettings = appSettings.Value;
        }

        public CarerService()
        {
        }

        public string Authenticate(string username, string password)
        {
            bool userPasswordCheck = CheckUsersPassword(username, password);

            // return null if user not found
            if (!userPasswordCheck)
                return null;

            var user = GetCarerByUsername(username);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName), 
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.Token;
        }

        public bool ValidateToken(string authToken)
        {
            try
            {
                var jwt = authToken.Replace("Bearer ", string.Empty);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();
                SecurityToken validatedToken;
                tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }


        public List<Carer> GetAllCarers() =>
           _carers.Find(user => true).ToList();

        public Carer GetActiveUser(string id) 
        {
           var user = _carers.Find(user => user.Id == id).FirstOrDefault();
           return user;
        }

        public Carer GetCarerByUsername(string username) =>
            _carers.Find(user => user.UserName == username).FirstOrDefault();

        public string GetCarerIdByUsername(string username)
        {
            try
            {
                return _carers.Find(user => user.UserName == username).FirstOrDefault().Id;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Carer CreateUser(NewUser user)
        {
            var newUserObj = ValidateNewUser(user);

            if (newUserObj != null)
            {
                var x = new Carer()
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                    Role = user.Role
                };
                _carers.InsertOne(x);
                return x;
            }
            return null;
        }

        public NewUser ValidateNewUser(NewUser user)
        {
            if (!UserExists(user))
            {
                try
                {
                    //Store a password hash:
                    PasswordHash hash = new PasswordHash(user.Password);
                    byte[] hashBytes = hash.ToArray();
                    user.Password = Convert.ToBase64String(hashBytes);
                    user.UserName.ToLower();
                    user.ConfirmPassword = null;
                    user.Role = "Carer";
                }
                catch (Exception ex)
                {
                    return null;
                }

                return user;
            }

            return null;
        }

        private bool UserExists(NewUser user)
        {
            try
            {
                return _carers.Find(c => c.UserName == user.UserName || c.EmailAddress == user.EmailAddress).Any();
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool CheckUsersPassword(string username, string password)
        {
            Carer userToCheck = null;

            try
            {
                userToCheck = _carers.Find(user => user.UserName == username).FirstOrDefault();
            }
            catch (Exception e)
            {
                userToCheck = null;
            }

            if (userToCheck != null)
            {
                // Check password against a stored hash
                byte[] hashBytes = Convert.FromBase64String(userToCheck.Password);
                PasswordHash hash = new PasswordHash(hashBytes);
                if (!hash.Verify(password))
                {
                    throw new UnauthorizedAccessException();
                }
                return true;
            }
            return false;
        }

        public void UpdateUser(string id, Carer updatedUser) =>
            _carers.ReplaceOne(user => user.Id == id, updatedUser);


        public bool AddDaysToRota(string carerId, WorkingDay[] daysToWork)
        {
            var carer = _carers.Find(c => c.Id == carerId).FirstOrDefault();//get the patient

            if (carer != null)
            {
                //update patients appointments in the database
               _carers.UpdateOneAsync(
                    Builders<Carer>.Filter.Eq(x => x.Id, carer.Id),
                    Builders<Carer>.Update.AddToSetEach(x => x.WorkingDays, daysToWork));

                return true;
            }

            return true;
        }

        public IEnumerable<WorkingDay> GetMyRota(string userId)
        {
            var user = _carers.Find(user => user.Id == userId).FirstOrDefault();
            var AllWorkingDays = user.WorkingDays != null ? user.WorkingDays.Where(c => true).ToList() : null;

            if (AllWorkingDays != null)
            {
                return AllWorkingDays.ToList();
            }

            return Enumerable.Empty<WorkingDay>();   
        }

    }

}
