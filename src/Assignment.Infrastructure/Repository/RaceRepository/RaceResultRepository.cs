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
    public class RaceResultRepository : IDBRaceResultRepository
    {
        private readonly RaceViewContext _context;

        public RaceResultRepository(RaceViewContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<List<RaceResult>> UpdateRaceResultAsync(List<RaceResult> raceResult)
        {
            await _context.RaceResults.AddRangeAsync(raceResult);
            await _context.SaveChangesAsync();
            return raceResult;
        }
        public async Task<List<PracticeResult>> UpdatePracticeResultsAsync(List<PracticeResult> practiceResults)
        {
            await _context.PracticeResults.AddRangeAsync(practiceResults);
            await _context.SaveChangesAsync();
            return practiceResults;
        }
        public async Task<List<DriverStanding>> UpdateDriverStanding(List<DriverStanding> driverStanding)
        {
            await _context.DriverStandings.AddRangeAsync(driverStanding);
            await _context.SaveChangesAsync();
            return driverStanding;
        }
        public async Task<List<TeamStanding>> UpdateTeamStanding(List<TeamStanding> teamStanding)
        {
            await _context.TeamStandings.AddRangeAsync(teamStanding);
            await _context.SaveChangesAsync();
            return teamStanding;
        }
        public async Task UpdateQualifyingResultAsync(List<QualifyingResult> qualifyingResults)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var result in qualifyingResults)
                    {
                        var existingResult = await _context.QualifyingResults.FirstOrDefaultAsync(qr => qr.RaceId == result.RaceId && qr.DriverId == result.DriverId);
                        if (existingResult != null)
                        {
                            existingResult.Q1 = result.Q1;
                            existingResult.Q2 = result.Q2;
                            existingResult.Q3 = result.Q3;
                            existingResult.Position = result.Position;
                            existingResult.LapsCompleted = result.LapsCompleted;
                            _context.QualifyingResults.Update(existingResult);
                        }
                        else
                        {
                            _context.QualifyingResults.Add(result);
                        }
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public async Task<QualifyingResult> GetQualifyingResultForDriversAsync(int raceId, int driverId, RoundType roundType)
        {
            return await _context.QualifyingResults.FirstOrDefaultAsync(q => q.RaceId == raceId && q.DriverId == driverId);
        }
        public async Task<List<QualifyingResult>> GetQualifyingResultAsync(int raceId)
        {
            var qualifyingResult = await _context.QualifyingResults.Include(rr => rr.Driver).Include(rr => rr.Team).Where(q => q.RaceId == raceId).ToListAsync();
            return qualifyingResult;
        }

        public async Task<List<RaceResult>> GetRaceResultsAsync(int raceId)
        {
            var results = await _context.RaceResults.Include(rr => rr.Driver).Include(rr => rr.Team).Where(rr => rr.RaceId == raceId).ToListAsync();
            return results;
        }

        public async Task<List<PracticeResult>> GetPracticeResultsAsync(int raceId, Practice roundType)
        {
            var results = await _context.PracticeResults.Include(rr => rr.Driver).Include(rr => rr.Team).Where(rr => rr.RaceId == raceId && rr.PracticeType == roundType.ToString()).ToListAsync();
            return results;
        }

        public async Task<List<RaceResult>> GetRaceResultsForRaces(List<int> raceIds)
        {
            return await _context.RaceResults
                .Include(r => r.Race)
                .Include(r => r.Driver)
                .Include(r => r.Team)
                .Where(r => raceIds.Contains(r.RaceId))
                .ToListAsync();
        }

        public async Task<RaceResult> GetFirstPlaceResultForRace(int raceId)
        {
            return await _context.RaceResults
                .Where(r => r.RaceId == raceId)
                .OrderBy(r => r.Position)
                .FirstOrDefaultAsync();
        }
    }
}
