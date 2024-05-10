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
    public class SeasonRepository : IDBSeasonRepository
    {
        private readonly RaceViewContext _context;

        public SeasonRepository(RaceViewContext dbContext) 
        {
            _context = dbContext;
        }

        public async Task<Season> AddSeasonAsync(Season season)
        {
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();
            return season;
        }

        public async Task AddDriverTeamAsync(DriverSeasonMapping driverTeamSeason)
        {
            _context.DriverSeasonMappings.Add(driverTeamSeason);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateDriverTeamAsync(DriverSeasonMapping driverTeamSeason)
        {
            _context.DriverSeasonMappings.Update(driverTeamSeason);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Season>> GetAllSeasonsAsync()
        {
            var result = await _context.Seasons.ToListAsync();
            return result;
        }

        public async Task<Season> GetSeasonIdByYearAsync(int year)
        {
            var season = await _context.Seasons.FirstOrDefaultAsync(s => s.Year == year);
            return season;
        }
        public async Task<Season> GetSeasonIdByIdAsync(int seasonId)
        {
            var season = await _context.Seasons.FirstOrDefaultAsync(s => s.SeasonId == seasonId);
            return season;
        }

        public async Task<DriverSeasonMapping> IsDriverMappedToSeasonAsync(int driverId, int seasonId)
        {
            var mappingExists = await _context.DriverSeasonMappings
                .FirstOrDefaultAsync(mapping => mapping.DriverId == driverId && mapping.IsActive == true && mapping.SeasonId == seasonId);

            return mappingExists;
        }

        public async Task<DriverSeasonMapping> IsDriverMappedToTeamAsync(int driverId, int teamId, int seasonId)
        {
            return await _context.DriverSeasonMappings.FirstOrDefaultAsync(mapping => mapping.DriverId == driverId && mapping.TeamId == teamId && mapping.IsActive && mapping.SeasonId == seasonId);
        }
        public async Task<List<DriverSeasonMapping>> TeamDriversBySeasonAsync(int seasonId)
        {
            var drivers = await _context.DriverSeasonMappings.Where(x => x.IsActive == true && x.SeasonId==seasonId).ToListAsync();
            return drivers;
        }

        public async Task<int> GetDriverCountForTeamAsync(int teamId,int seasonId)
        {
            return await _context.DriverSeasonMappings
                .CountAsync(dsm => dsm.TeamId == teamId && dsm.SeasonId == seasonId && dsm.IsActive == true);
        }

        public async Task<List<DriverSeasonMapping>> GetDriversForTeamAsync(int teamId, int seasonId)
        {
            var result =  await _context.DriverSeasonMappings.Include(dsm => dsm.Driver).Where(dsm => dsm.TeamId == teamId && dsm.IsActive == true && dsm.SeasonId== seasonId).ToListAsync();
            return result;
        }

        public async Task<Season> GetCurrentSeasonAsync()
        {
            int currentYear = DateTime.UtcNow.Year;
            var result =  await _context.Seasons.Where(season => season.Year == currentYear).FirstOrDefaultAsync();
            return result;
        }

    }
}
