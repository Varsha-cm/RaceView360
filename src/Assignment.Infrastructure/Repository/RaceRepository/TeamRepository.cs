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
    public class TeamRepository : IDBTeamRepository
    {
        private readonly RaceViewContext _context;
        public TeamRepository(RaceViewContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<Team> AddTeam(Team team)
        {
            await _context.AddAsync(team);
            _context.SaveChanges();
            return team;
        }
        public async Task<List<Team>> GetAllTeams()
        {
            return await _context.Teams.ToListAsync();
        }
        public async Task<Team> GetTeamByCode(string teamCode)
        {
            var result = await _context.Teams.FirstOrDefaultAsync(x => x.TeamCode == teamCode);
            return result;
        }
        public async Task<Team> GetTeamById(int teamId)
        {
            var result = await _context.Teams.FirstOrDefaultAsync(x => x.TeamId == teamId);
            return result;
        }
        public async Task<bool> IsTeamNameExistsAsync(string teamName)
        {
            return await _context.Teams.AnyAsync(t => t.TeamName == teamName);
        }


    }
}
