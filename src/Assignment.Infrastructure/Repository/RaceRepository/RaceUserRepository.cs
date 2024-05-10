using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository.RaceRepository
{
    public class RaceUserRepository : IDBRaceUserRepository
    {
        private readonly RaceViewContext _context;

        public RaceUserRepository(RaceViewContext raceViewContext) 
        {
            _context = raceViewContext;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(r => r.Email == email);
        }
        public async Task<int> AuthenticateAsync(string email, string password)
        {
            var output = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (output == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            var ret = await _context.Users.Where(r => r.UserId == output.UserId).Select(e => e.RoleId).FirstOrDefaultAsync();
            return ret;
        }

    }
}
