using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Assignment.Api.Interfaces;
using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository
{
    public class AuthRepository : IDBAuthRepository
    {
        private readonly RaidenDBContext _dbContext;

        public AuthRepository(RaidenDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AuthenticateAsync(string email, string password)
        {
            var output = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if(output == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            var ret =await  _dbContext.UserRoles
         .Where(r => r.UserId == output.UserId).Select(e=>e.RoleId).FirstOrDefaultAsync();
            return ret;
        }

        public async Task <int> IsRefreshTokenValid(string refreshToken, string username)
        {
            // Check if the refresh token exists in the database
            var existingToken = _dbContext.Users
                .SingleOrDefault(t => t.RefreshToken == refreshToken);

            if (existingToken == null)
            {
                return 0;
            }

            if (existingToken.TokenExpiryTime >= DateTime.UtcNow)
            {
                return 0;
            }

            // Check if the token is associated with the provided username
            if (existingToken.Email != username)
            {
                return 0; // Token doesn't match the user
            }

            return 1;
        }

        public async Task SaveRefreshToken(string username, string refreshToken)
        {
            var existingToken = await _dbContext.Users.SingleOrDefaultAsync(t => t.Email == username);

            existingToken.RefreshToken = refreshToken;
            existingToken.TokenExpiryTime = DateTime.UtcNow;
            _dbContext.Users.Update(existingToken);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetRefreshToken(string username)
        {
            var token = await _dbContext.Users.SingleOrDefaultAsync(t => t.Email == username);
            return token?.RefreshToken;
        }

        public async Task RemoveRefreshToken(string username)
        {
            var token = await _dbContext.Users.SingleOrDefaultAsync(t => t.Email == username);
            if (token != null)
            {
                _dbContext.Users.Remove(token);
                await _dbContext.SaveChangesAsync();
            }
        }

        public int AuthenticateApplication(string clientID, string clientSecret)
        {
            var application = _dbContext.Applications
                .FirstOrDefault(a => a.ClientId == clientID && a.ClientSecret == clientSecret);

            return application != null ? application.ApplicationId : 0;
        }

    }
}
