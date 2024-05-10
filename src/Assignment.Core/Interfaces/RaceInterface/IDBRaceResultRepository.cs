using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBRaceResultRepository
    {
        Task<List<PracticeResult>> UpdatePracticeResultsAsync(List<PracticeResult> practiceResults);
        Task UpdateQualifyingResultAsync(List<QualifyingResult> qualifyingResult);
        Task<List<RaceResult>> UpdateRaceResultAsync(List<RaceResult> raceResult);
        Task<QualifyingResult> GetQualifyingResultForDriversAsync(int raceId, int driverId, RoundType roundType);
        Task<List<QualifyingResult>> GetQualifyingResultAsync(int raceId);
        Task<List<DriverStanding>> UpdateDriverStanding(List<DriverStanding> driverStanding);
        Task<List<TeamStanding>> UpdateTeamStanding(List<TeamStanding> teamStanding);
        Task<List<RaceResult>> GetRaceResultsAsync(int raceId);
        Task<List<PracticeResult>> GetPracticeResultsAsync(int raceId, Practice roundType);
        Task<List<RaceResult>> GetRaceResultsForRaces(List<int> raceIds);
        Task<RaceResult> GetFirstPlaceResultForRace(int raceId);

    }
}
