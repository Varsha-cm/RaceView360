using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services.RaceService
{
    public class SeasonService
    {
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBTeamRepository _teamRepository;
        private readonly IDBDriverRepository _driverRepository;

        public SeasonService(IDBSeasonRepository dBSeasonRepository, IDBDriverRepository dBDriverRepository,IDBTeamRepository dBTeamRepository) 
        {
            _seasonRepository = dBSeasonRepository;
            _teamRepository = dBTeamRepository;
            _driverRepository = dBDriverRepository;
        }
        public async Task<SeasonModel> AddSeasonAsync(SeasonModel model)
        {
            var existingSeason = await _seasonRepository.GetSeasonIdByYearAsync(model.Year);
            if(existingSeason != null)
            {
                throw new Exception($"Season with year {model.Year} already exist");
            }
            var season = new Season
            {
                Year = model.Year,
                Url = model.Url
            };

            var createdSeason = await _seasonRepository.AddSeasonAsync(season);
            return new SeasonModel
            {
                Year = createdSeason.Year,
                Url = createdSeason.Url
            };
        }

        public async Task<List<SeasonModel>> GetAllSeasonAsync()
        {
            try
            {
                var result = await _seasonRepository.GetAllSeasonsAsync();
                var seasons = new List<SeasonModel>();
                foreach (var season in result)
                {

                    var response = new SeasonModel
                    {
                        Year=season.Year,
                        Url = season.Url
                    };
                    seasons.Add(response);
                }
                return seasons;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<SeasonModel> GetSeasonByYearAsync(int year)
        {
            try
            {
                var result = await _seasonRepository.GetSeasonIdByYearAsync(year);
                if (result == null)
                {
                    throw new ArgumentException($"No season Found with year '{year}'");
                }
                var response = new SeasonModel
                {
                    Year = result.Year,
                    Url = result.Url
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task MapDriverTeamAsync(DriverTeamMappingModel model)
        {
            var driver = await _driverRepository.GetDriverByCode(model.DriverCode);
            var team = await _teamRepository.GetTeamByCode(model.TeamCode);
            var season = await _seasonRepository.GetSeasonIdByYearAsync(model.Season);
            if(season == null)
            {
                throw new ArgumentException($"No season Found with year '{model.Season}'");
            }
            if (driver == null)
            {
                throw new ArgumentException($"No driver Found with driverCode '{model.DriverCode}'");
            }
            if (team == null)
            {
                throw new ArgumentException($"No Team Found with teamCode '{model.TeamCode}'");
            }
            var isDriverMapped = await _seasonRepository.IsDriverMappedToSeasonAsync(driver.DriverId, season.SeasonId);
            if (isDriverMapped != null)
            {
                throw new ArgumentException($"Driver with code {model.DriverCode} is already mapped to another team.");
            }
            var teamDriverCount = await _seasonRepository.GetDriverCountForTeamAsync(team.TeamId, season.SeasonId);
            if (teamDriverCount >= 2)
            {
                throw new ArgumentException($"Team with code {model.TeamCode} already has two drivers mapped.");
            }

            DriverSeasonMapping driverTeamSeason = new DriverSeasonMapping
            {
                DriverId = driver.DriverId,
                TeamId = team.TeamId,
                IsActive = true,
                SeasonId = season.SeasonId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _seasonRepository.AddDriverTeamAsync(driverTeamSeason);
        }

        public async Task UnmapDriverTeamAsync(DriverTeamMappingModel model)
        {
            var driver = await _driverRepository.GetDriverByCode(model.DriverCode);
            var team = await _teamRepository.GetTeamByCode(model.TeamCode);
            var season = await _seasonRepository.GetSeasonIdByYearAsync(model.Season);
            if (season == null)
            {
                throw new ArgumentException($"No season Found with year '{model.Season}'");
            }
            if (driver == null)
            {
                throw new ArgumentException($"No driver Found with driverCode '{model.DriverCode}'");
            }
            if (team == null)
            {
                throw new ArgumentException($"No Team Found with teamCode '{model.TeamCode}'");
            }
            var isDriverMapped = await _seasonRepository.IsDriverMappedToTeamAsync(driver.DriverId, team.TeamId, season.SeasonId);
            if (isDriverMapped == null)
            {
                throw new ArgumentException($"Driver with code {model.DriverCode} is not mapped to team with code {model.TeamCode}.");
            }

            var teamDriverCount = await _seasonRepository.GetDriverCountForTeamAsync(team.TeamId,season.SeasonId);
            if (teamDriverCount == 0 )
            {
                throw new ArgumentException($"Team with code {model.TeamCode} has no drivers");
            }
            if (isDriverMapped != null)
            {
                isDriverMapped.IsActive = false; 
                await _seasonRepository.UpdateDriverTeamAsync(isDriverMapped);
            }
        }

    }
}
