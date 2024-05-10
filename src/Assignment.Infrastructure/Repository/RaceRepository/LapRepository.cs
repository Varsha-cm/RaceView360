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
    public class LapRepository : IDBLapsRepository
    {
        private readonly RaceViewContext _context;

        public LapRepository(RaceViewContext dbcontext)
        {
            _context = dbcontext;
        }
        public async Task<LapTime> AddLiveRace(LapTime lapTime)
        {
            _context.LapTimes.Add(lapTime);
            await _context.SaveChangesAsync();
            return lapTime;
        }

        public async Task<List<LapTime>> GetLapsForRaceAndRoundType(int raceId, RoundType roundType)
        {
            return await _context.LapTimes.Where(l => l.RaceId == raceId && l.RoundType == roundType).ToListAsync();
        }

    }
}
