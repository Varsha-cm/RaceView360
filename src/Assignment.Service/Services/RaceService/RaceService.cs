using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services.RaceService
{
    public class RaceService
    {
        private readonly IDBRaceRepository _raceRepository;
        private readonly IDBCircuitRepository _circuitRepository;
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBRaceResultRepository _raceResultRepository;

        public RaceService(IDBRaceResultRepository dBRaceResultRepository,IDBRaceRepository dBRaceRepository, IDBCircuitRepository dBCircuitRepository, IDBSeasonRepository dBSeasonRepository)
        {
            _raceRepository = dBRaceRepository;
            _circuitRepository = dBCircuitRepository;
            _seasonRepository = dBSeasonRepository;
            _raceResultRepository = dBRaceResultRepository;
        }
        public async Task<RaceModel> AddRaceAsync(RaceModel model)
        {
            try
            {
                var circuit = await _circuitRepository.GetByCodeAsync(model.CircuitCode);
                var season = await _seasonRepository.GetSeasonIdByYearAsync(model.Year);
                if(circuit == null)
                {
                    throw new ArgumentException($"No circuit with circuitCode {model.CircuitCode} found");
                }
                if (season == null)
                {
                    throw new ArgumentException($"No season found for year {model.Year}");
                } 
                var existingRace = await _raceRepository.GetRaceByCodeAsync(model.RaceCode);
                if(existingRace != null)
                {
                    throw new ArgumentException($"race with race code {model.RaceCode} already exist");

                }
                var race = new Race
                {
                    RaceCode = model.RaceCode,
                    RaceName = model.RaceName,
                    CircuitId = circuit.CircuitId,
                    SeasonId = season.SeasonId,
                    Status = Status.Upcoming,
                    RaceDateTime = model.RaceDateTime,
                    Practice1DateTime = model.Practice1DateTime,
                    Practice2DateTime = model.Practice2DateTime,
                    Practice3DateTime = model.Practice3DateTime,
                    QualifyDateTime = model.QualifyDateTime,
                    Url = model.Url,
                };

                var createdRace = await _raceRepository.AddRaceAsync(race);

                var responseModel = new RaceModel
                {
                    RaceCode = createdRace.RaceCode,
                    RaceName = createdRace.RaceName,
                    CircuitCode = model.CircuitCode,
                    Year = model.Year,
                    RaceDateTime = createdRace.RaceDateTime,
                    Practice1DateTime = createdRace.Practice1DateTime,
                    Practice2DateTime = createdRace.Practice2DateTime,
                    Practice3DateTime = createdRace.Practice3DateTime,
                    QualifyDateTime = createdRace.QualifyDateTime,
                    Url = createdRace.Url,
                };

                return responseModel;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }


        public async Task<RaceModel> UpdateRaceAsync(string raceCode,UpdateRaceModel model)
        {
            try
            {
                var circuit = await _circuitRepository.GetByCodeAsync(model.CircuitCode);
                if (circuit == null)
                {
                    throw new ArgumentException($"No circuit with circuitCode {model.CircuitCode} found");
                }
                var existingRace = await _raceRepository.GetRaceByCodeAsync(raceCode);
                if (existingRace == null)
                {
                    throw new ArgumentException($"Race with race code {raceCode} doesn't exist");
                }
                if (existingRace.Status == Status.Completed)
                {
                    throw new ArgumentException($"Race with race code {raceCode} has already completed");
                }
                existingRace.RaceName = model.RaceName;
                existingRace.CircuitId = circuit.CircuitId;
                existingRace.Status = Status.Upcoming;
                existingRace.RaceDateTime = model.RaceDateTime;
                existingRace.Practice1DateTime = model.Practice1DateTime;
                existingRace.Practice2DateTime = model.Practice2DateTime;
                existingRace.Practice3DateTime = model.Practice3DateTime;
                existingRace.QualifyDateTime = model.QualifyDateTime;
                existingRace.Url = model.Url;
                var updatedRace = await _raceRepository.UpdateRace(existingRace);
                var circuitDetails = await _circuitRepository.GetCircuitByIdAsync(updatedRace.CircuitId);
                var seasonDetails = await _seasonRepository.GetSeasonIdByIdAsync(updatedRace.SeasonId);
                var responseModel = new RaceModel
                {
                    RaceCode = updatedRace.RaceCode,
                    RaceName = updatedRace.RaceName,
                    CircuitCode = circuitDetails.CircuitCode,
                    Year = seasonDetails.Year,
                    RaceDateTime = updatedRace.RaceDateTime,
                    Practice1DateTime = updatedRace.Practice1DateTime,
                    Practice2DateTime = updatedRace.Practice2DateTime,
                    Practice3DateTime = updatedRace.Practice3DateTime,
                    QualifyDateTime = updatedRace.QualifyDateTime,
                    Url = updatedRace.Url
                };

                return responseModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<RaceModelAll>> GetAllRacesForSeason(int year)
        {
            var season = await _seasonRepository.GetSeasonIdByYearAsync(year);
            if (season == null)
            {
                throw new ArgumentException($"No season found for year {year}");
            }
            var races = await _raceRepository.GetAllRacesForSeason(season.SeasonId);
            var raceModels = new List<RaceModelAll>();

            foreach (var race in races)
            {
                var circuit = await _circuitRepository.GetCircuitByIdAsync(race.CircuitId);
                if (circuit == null)
                {
                    continue;
                }

                var raceModel = new RaceModelAll
                {
                    RaceCode = race.RaceCode,
                    RaceName = race.RaceName,
                    CircuitCode = circuit.CircuitCode,
                };

                raceModels.Add(raceModel);
            }

            return raceModels;
        }

        public async Task<RaceModel> GetRaceForSeason(string raceCode)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No race found with aceCode {raceCode}");
            }
            var circuit = await _circuitRepository.GetCircuitByIdAsync(race.CircuitId);
            var season = await _seasonRepository.GetSeasonIdByIdAsync(race.SeasonId);
            var raceModel = new RaceModel
            {
                RaceCode = race.RaceCode,
                RaceName = race.RaceName,
                CircuitCode = circuit.CircuitCode,
                Year = season.Year,
                RaceDateTime = race.RaceDateTime,
                Practice1DateTime = race.Practice1DateTime,
                Practice2DateTime = race.Practice2DateTime,
                Practice3DateTime = race.Practice3DateTime,
                QualifyDateTime = race.QualifyDateTime,
                Url = race.Url, 
            };
            return raceModel;
        }

        public async Task<PreviousRaceResultModel> GetPreviousRaceAsync()
        {
            var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
            if (currentSeason == null)
            {
                throw new ArgumentException($"No data Found for current season");
            }
            var previousRace = await _raceRepository.GetPreviousRaceAsync(currentSeason.SeasonId);
            if (previousRace == null)
            {
                throw new ArgumentException("No previous race found.");
            }
            var raceResult = await _raceResultRepository.GetRaceResultsAsync(previousRace.RaceId);
            var season = await _seasonRepository.GetSeasonIdByIdAsync(previousRace.SeasonId);

            var result = new PreviousRaceResultModel
            {
                RaceCode = previousRace.RaceCode,
                RaceName = previousRace.RaceName,
                CircuitCode = previousRace.Circuit.CircuitCode,
                Year = season.Year,
                RaceDateTime = previousRace.RaceDateTime,
                Practice1DateTime = previousRace.Practice1DateTime,
                Practice2DateTime = previousRace.Practice2DateTime,
                Practice3DateTime = previousRace.Practice3DateTime,
                QualifyDateTime = previousRace.QualifyDateTime,
                Position1 = raceResult.Count > 0 ? raceResult[0].Driver.FirstName + " " + raceResult[0].Driver.LastName : "N/A",
                Position2 = raceResult.Count > 1 ? raceResult[1].Driver.FirstName + " " + raceResult[1].Driver.LastName : "N/A",
                Position3 = raceResult.Count > 2 ? raceResult[2].Driver.FirstName + " " + raceResult[2].Driver.LastName : "N/A"
            };
            return result;
        }

        public async Task<RaceModel> GetRaceInprogressAsync()
        {
            var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
            if (currentSeason == null)
            {
                throw new ArgumentException($"No data Found for current season");
            }
            var nextRace = await _raceRepository.GetInProgressRaceAsync(currentSeason.SeasonId);
            if (nextRace == null)
            {
                throw new ArgumentException("No race is in progress.");
            }
            return await MapToModel(nextRace);
        }

        public async Task<RaceModel> MapToModel(Race race)
        {
            var season = await _seasonRepository.GetSeasonIdByIdAsync(race.SeasonId);
            return new RaceModel
            {
                RaceCode = race.RaceCode,
                RaceName = race.RaceName,
                CircuitCode = race.Circuit.CircuitCode,
                Year = season.Year,
                RaceDateTime = race.RaceDateTime,
                Practice1DateTime = race.Practice1DateTime,
                Practice2DateTime = race.Practice2DateTime,
                Practice3DateTime = race.Practice3DateTime,
                QualifyDateTime = race.QualifyDateTime,
                Url = race.Url,
            };
        }

        public async Task<List<RaceModel>> GetUpcomingRacesAsync(int count)
        {
            var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
            if (currentSeason == null)
            {
                throw new ArgumentException($"No data Found for current season");
            }
            var upcomingRaces = await _raceRepository.GetUpcomingRacesAsync(currentSeason.SeasonId, count);
            if (upcomingRaces == null || !upcomingRaces.Any())
            {
                throw new ArgumentException("No upcoming races found.");
            }
            var raceModelTasks = upcomingRaces.Select(MapToModel);
            var raceModels = await Task.WhenAll(raceModelTasks);
            return raceModels.ToList();
        }



    }
}
