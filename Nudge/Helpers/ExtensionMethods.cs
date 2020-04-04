using ShareCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareCare.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<Carer> WithoutPasswords(this IEnumerable<Carer> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static Carer WithoutPassword(this Carer user)
        {
            user.Password = null;
            return user;
        }
    }
}
