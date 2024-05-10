using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services.RaceService
{
    public class RaceResultService
    {
        private readonly IDBRaceRepository _raceRepository;
        private readonly IDBDriverRepository _driverRepository;
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBLapsRepository _lapRepository;
        private readonly IDBRoundRepository _roundRepository;
        private readonly IDBRaceResultRepository _raceResultRepository;
        private readonly IDBTeamRepository _teamRepository;

        public RaceResultService(IDBTeamRepository dBTeamRepository, IDBRoundRepository dBRoundRepository, IDBRaceResultRepository dBRaceResultRepository, IDBRaceRepository dBRaceRepository, IDBLapsRepository dBLapRepository, IDBDriverRepository dBDriverRepository, IDBSeasonRepository dbSeasonRepository)
        {
            _raceRepository = dBRaceRepository;
            _driverRepository = dBDriverRepository;
            _seasonRepository = dbSeasonRepository;
            _lapRepository = dBLapRepository;
            _roundRepository = dBRoundRepository;
            _raceResultRepository = dBRaceResultRepository;
            _teamRepository = dBTeamRepository;
        }

        public async Task<List<RaceResultResponse>> GetRaceResultsAsync(int season, string raceCode)
        {
            var seasonDetails = await _seasonRepository.GetSeasonIdByYearAsync(season);
            if (seasonDetails == null)
            {
                throw new ArgumentException($"No season found for year {season}");
            }
            var race = await _raceRepository.GetRaceForSeason(seasonDetails.SeasonId, raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No race with {raceCode} found for the season {season}");
            }
            var raceResults = await _raceResultRepository.GetRaceResultsAsync(race.RaceId);
            if(raceResults.Count() == 0)
            {
                throw new ArgumentException($"Data not found");
            }
            var response = raceResults.Select(result => new RaceResultResponse
            {
                Position = result.Position,
                Number = result.Driver.DriverNumber,
                DriverName = result.Driver.FirstName + " " + result.Driver.LastName,
                TeamName = result.Team.TeamName,
                Laps = result.LapsCompleted,
                FastestLapTime = result.FastestLapTime,
                TimeOrRetired = result.FinishingTime,
                GridPosition = result.GridPosition,
                Points = result.PointsEarned
            }).ToList();
            response = response.OrderBy(x => x.Position).ToList();
            return response;
        }

        public async Task<List<PracticeResultResponse>> GetPracticeResultsAsync(int season, string raceCode, Practice roundType)
        {
            var seasonDetails = await _seasonRepository.GetSeasonIdByYearAsync(season);
            if (seasonDetails == null)
            {
                throw new ArgumentException($"No season found for year {season}");
            }
            var race = await _raceRepository.GetRaceForSeason(seasonDetails.SeasonId, raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No race with {raceCode} found for the season {season}");
            }
            var raceResults = await _raceResultRepository.GetPracticeResultsAsync(race.RaceId, roundType );
            if (raceResults.Count() == 0)
            {
                throw new ArgumentException($"Data not found");
            }
            var response = raceResults.Select(result => new PracticeResultResponse
            {
                Position = result.Position,
                Number = result.Driver.DriverNumber,
                DriverName = result.Driver.FirstName + " " + result.Driver.LastName,
                TeamName = result.Team.TeamName,
                Time = result.FastestLapTime,
                Laps = result.LapsCompleted
            }).ToList();
            response = response.OrderBy(x => x.Position).ToList();
            return response;
        }

        public async Task<List<QualifyResultResponse>> GetQualifyingResultsAsync(int season, string raceCode)
        {
            var seasonDetails = await _seasonRepository.GetSeasonIdByYearAsync(season);
            if (seasonDetails == null)
            {
                throw new ArgumentException($"No season found for year {season}");
            }
            var race = await _raceRepository.GetRaceForSeason(seasonDetails.SeasonId, raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No race with {raceCode} found for the season {season}");
            }
            var raceResults = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
            if (raceResults.Count() == 0)
            {
                throw new ArgumentException($"Data not found");
            }
            var response = raceResults.Select(result => new QualifyResultResponse
            {
                Position = result.Position,
                Number = result.Driver.DriverNumber,
                DriverName = result.Driver.FirstName + " " + result.Driver.LastName,
                TeamName = result.Team.TeamName,
                Q1 = result.Q1,
                Q2 = result.Q2,
                Q3 = result.Q3,
                Laps = result.LapsCompleted,
            }).ToList();

            response = response.OrderBy(x => x.Position).ToList();  

            return response;
        }

        public async Task<List<RaceResultModel>> GetRaceResultsForRaces(int season)
        {
            var seasonDetails = await _seasonRepository.GetSeasonIdByYearAsync(season);
            if (seasonDetails == null)
            {
                throw new ArgumentException($"No season found for year {season}");
            }
            var races = await _raceRepository.GetAllRacesForSeason(seasonDetails.SeasonId);
            if (races.Count() == 0)
            {
                throw new ArgumentException($"Data not found");
            }
            var raceIds = races.Select(r => r.RaceId).ToList();
            var raceResultResponse = new List<RaceResultModel>();
            foreach (var raceId in raceIds)
            {
                var firstPlaceResult = await _raceResultRepository.GetFirstPlaceResultForRace(raceId);
                if (firstPlaceResult != null)
                {
                    var race = await _raceRepository.GetRaceByIdAsync(raceId);
                    var winnerDriver = await _driverRepository.GetDriverById(firstPlaceResult.DriverId);
                    var team = await _teamRepository.GetTeamById(firstPlaceResult.TeamId);
                    var model = new RaceResultModel
                    {
                        GrandPrix = race.RaceName,
                        Date = race.RaceDateTime,
                        Winner = $"{winnerDriver.FirstName} {winnerDriver.LastName}",
                        Car = team.TeamName,
                        Laps = firstPlaceResult.LapsCompleted,
                        Time = firstPlaceResult.FinishingTime,
                    };

                    raceResultResponse.Add(model);
                }
            }
            return raceResultResponse;
        }
    }
}
