using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBRaceUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<int> AuthenticateAsync(string email, string password);
    }
}
