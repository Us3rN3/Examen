using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VivesRental.Domains.EntitiesDB;

namespace VivesRental.Services.Interfaces
{
    public interface IUserService
    {
        User? ValidateUser(string username, string password);
    }
}

