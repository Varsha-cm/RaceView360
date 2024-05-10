using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository.RaceRepository
{
    public class RaceRepository : IDBRaceRepository
    {
        private readonly RaceViewContext _context;
        public RaceRepository(RaceViewContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<Race> AddRaceAsync(Race race)
        {
            _context.Races.Add(race);
            await _context.SaveChangesAsync();
            return race;
        }

        public async Task<Race> UpdateRaceAsync(Race race)
        {
            _context.Races.Update(race);
            await _context.SaveChangesAsync();
            return race;
        }

        public async Task<Race> GetRaceByCodeAsync(string raceCode)
        {
            return await _context.Races.FirstOrDefaultAsync(x => x.RaceCode == raceCode);
        }
        public async Task<Race> GetRaceByIdAsync(int raceId)
        {
            return await _context.Races.FirstOrDefaultAsync(x => x.RaceId == raceId);
        }

        public async Task<List<Race>> GetAllRacesForSeason(int seasonId)
        {
            var races = await _context.Races.Where(x => x.SeasonId == seasonId).ToListAsync();
            return races;
        }

        public async Task<Race> GetRaceForSeason(int seasonId, string raceCode)
        {
            var races = await _context.Races.FirstOrDefaultAsync(x => x.SeasonId == seasonId && x.RaceCode == raceCode);
            return races;
        }

        public async Task<List<Race>> GetRaceScheduleAsync(Status status)
        {
            var currentYear = DateTime.Today.Year;
            var pastRaces = await _context.Races.Where(r => r.Status == status && r.RaceDateTime < DateTime.Today && r.Season.Year == currentYear).ToListAsync();
            return pastRaces;
        }

        public async Task<Race> UpdateRace(Race race)
        {
            _context.Races.Update(race);
            await _context.SaveChangesAsync();
            return race;
        }

        public async Task<List<Race>> GetRacesByStatusAsync(int seasonId, Status status)
        {
            return await _context.Races.Where(race => race.SeasonId == seasonId && race.Status == status).ToListAsync();
        }
        public async Task<Race> GetPreviousRaceAsync(int currentSeason)
        {
            return await _context.Races.Include(x => x.Circuit)
                .Where(race =>race.SeasonId == currentSeason && race.Status == Status.Completed)
                .OrderByDescending(race => race.RaceDateTime)
                .FirstOrDefaultAsync();
        }

        public async Task<Race> GetInProgressRaceAsync(int currentSeason)
        {
            return await _context.Races
                .Include(x => x.Circuit)
                .Where(race => race.SeasonId == currentSeason && race.Status == Status.Inprogress)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Race>> GetUpcomingRacesAsync(int currentSeason, int count)
        {
            return await _context.Races.Include(x => x.Circuit)
                .Where(race => race.SeasonId == currentSeason && race.Status== Status.Upcoming)
                .OrderBy(race => race.RaceDateTime)
                .Take(count)
                .ToListAsync();
        }

    }
}
