using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository.RaceRepository
{
    public class RoundRepository : IDBRoundRepository
    {
        private readonly RaceViewContext _context;

        public RoundRepository(RaceViewContext context)
        {
            _context = context;
        }

        public async Task<List<Round>> GetRoundsByRaceId(int raceId)
        {
            return await _context.Rounds.Where(r => r.RaceId == raceId).ToListAsync();
        }

        public async Task<Round> AddRoundDetails(Round round)
        {
            await _context.Rounds.AddAsync(round);
            await _context.SaveChangesAsync();
            return round;
        }
        public async Task<Round> UpdateRoundDetails(Round round)
        {
            _context.Entry(round).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return round;
        }

    }
}
