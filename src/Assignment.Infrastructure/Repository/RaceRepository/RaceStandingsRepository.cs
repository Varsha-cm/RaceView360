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
    public class RaceStandingsRepository : IDBRaceStandingsRepository
    {
        private readonly RaceViewContext _context;

        public RaceStandingsRepository(RaceViewContext context)
        {
            _context = context;
        }
        public async Task<List<DriverStanding>> GetDriverStandingsByDriverIdAsync(int driverId)
        {
            return await _context.DriverStandings.Where(ds => ds.DriverId == driverId).Include(ds => ds.Race).ThenInclude(race => race.RaceResults).ToListAsync();
        }
        public async Task<List<TeamStanding>> GetTeamStandingsByTeamIdAsync(int teamId)
        {
            return await _context.TeamStandings.Include(ds => ds.Race).Where(ds => ds.TeamId == teamId).ToListAsync();
        }
        public async Task<List<DriverStanding>> GetDriverStandingsForYear(int seasonId, List<int> raceIds)
        {
            return await _context.DriverStandings.Include(ds => ds.Driver).Include(ds => ds.Race).Include(ds => ds.Team)
                .Where(ds => ds.Race.SeasonId == seasonId && raceIds.Contains(ds.RaceId)).OrderBy(ds => ds.Position).ToListAsync();
        }

        public async Task<List<DriverStanding>> GetDriverStandingsForRaces(int seasonId, int driverId)
        {
            return await _context.DriverStandings.Include(ds => ds.Driver).Include(ds => ds.Race).Include(ds => ds.Team)
                .Where(ds => ds.Race.SeasonId == seasonId && ds.DriverId == driverId).OrderBy(ds => ds.RaceId).ToListAsync();
        }
        public async Task<List<TeamStanding>> GetTeamStandingsForYear(int seasonId, List<int> raceIds)
        {
            return await _context.TeamStandings.Include(ds => ds.Team).Include(ds => ds.Race)
                .Where(ds => ds.Race.SeasonId == seasonId && raceIds.Contains(ds.RaceId)).OrderBy(ds => ds.Position).ToListAsync();
        }


        public async Task<List<TeamStanding>> GetTeamStandingsForRaces(int seasonId, int teamId)
        {
            return await _context.TeamStandings.Include(ts => ts.Race).Where(ts => ts.Race.SeasonId == seasonId && ts.TeamId == teamId).OrderBy(ts => ts.RaceId).ToListAsync();
        }

    }
}
