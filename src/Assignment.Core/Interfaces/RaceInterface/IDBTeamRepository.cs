using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBTeamRepository
    {
        Task<Team> AddTeam(Team team);
        Task<List<Team>> GetAllTeams();
        Task<Team> GetTeamByCode(string teamCode);
        Task<Team> GetTeamById(int teamId);
        Task<bool> IsTeamNameExistsAsync(string teamName);

    }
}
