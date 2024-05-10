using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBRaceStandingsRepository
    {
        Task<List<DriverStanding>> GetDriverStandingsByDriverIdAsync(int driverId);
        Task<List<TeamStanding>> GetTeamStandingsByTeamIdAsync(int teamId);
        Task<List<DriverStanding>> GetDriverStandingsForYear(int seasonId, List<int> raceIds);
        Task<List<DriverStanding>> GetDriverStandingsForRaces(int seasonId, int driverId);
        Task<List<TeamStanding>> GetTeamStandingsForYear(int seasonId, List<int> raceIds);
        Task<List<TeamStanding>> GetTeamStandingsForRaces(int seasonId, int driverId);
    }
}
