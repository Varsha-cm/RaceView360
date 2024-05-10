using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces
{
    public interface IDBAuthRepository
    {
        public Task<int> AuthenticateAsync(string email, string password);
        public Task SaveRefreshToken(string username, string refreshToken);
        public Task<string> GetRefreshToken(string username);
        public Task RemoveRefreshToken(string username);
        public Task<int> IsRefreshTokenValid(string refreshToken, string username);
        public int AuthenticateApplication(string clientID, string clientSecret);

        

    }
}
