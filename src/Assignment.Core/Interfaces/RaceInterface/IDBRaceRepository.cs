using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBRaceRepository
    {
        Task<Race> AddRaceAsync(Race race);
        Task<Race> UpdateRaceAsync(Race race);
        Task<List<Race>> GetAllRacesForSeason(int seasonId);
        Task<Race> GetRaceByCodeAsync(string raceCode);
        Task<Race> GetRaceByIdAsync(int raceId);
        Task<Race> GetRaceForSeason(int seasonId, string raceCode);
        Task<List<Race>> GetRaceScheduleAsync(Status status);
        Task<Race> UpdateRace(Race race);
        Task<List<Race>> GetRacesByStatusAsync(int seasonId, Status status);
        Task<List<Race>> GetUpcomingRacesAsync(int currentSeason, int count);
        Task<Race> GetInProgressRaceAsync(int currentSeason);
        Task<Race> GetPreviousRaceAsync(int currentSeason);
    }
}
