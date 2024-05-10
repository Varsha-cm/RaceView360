using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services.RaceService
{
    public class TeamService
    {
        private readonly IDBTeamRepository _teamRepository;
        private readonly IDBRaceStandingsRepository _raceStandingsRepository;
        private readonly IDBRaceRepository _raceRepository;
        private readonly IDBRaceResultRepository _raceResultRepository;
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBDriverRepository _driverRepository;

        public TeamService(IDBDriverRepository driverRepository,IDBSeasonRepository dBSeasonRepository,IDBRaceResultRepository dBRaceResultRepository,IDBRaceRepository dBRaceRepository,IDBRaceStandingsRepository dBRaceStandingsRepository,IDBTeamRepository dBTeamRepository)
        {
            _teamRepository = dBTeamRepository;
            _raceStandingsRepository = dBRaceStandingsRepository;
            _raceRepository = dBRaceRepository;
            _raceResultRepository = dBRaceResultRepository;
            _seasonRepository = dBSeasonRepository;
            _driverRepository = driverRepository;
        }
        public async Task<List<TeamModel>> GetAllTeamAsync()
        {
            try
            {
                var result = await _teamRepository.GetAllTeams();
                var teams = new List<TeamModel>();
                var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
                foreach (var team in result)
                {
                    var currentDrivers = new List<string>(); 
                    var driverSeasonMappings = await _seasonRepository.GetDriversForTeamAsync(team.TeamId,currentSeason.SeasonId);
                    foreach (var mapping in driverSeasonMappings)
                    {
                        var driver = await _driverRepository.GetDriverById(mapping.DriverId);
                        if (driver != null)
                        {
                            currentDrivers.Add($"{driver.FirstName} {driver.LastName}");
                        }
                    }

                    var response = new TeamModel
                    {
                        TeamCode = team.TeamCode,
                        TeamName = team.TeamName,
                        Nationality = team.Nationality,
                        Url = team.Url,
                    };
                    teams.Add(response);
                }
                return teams;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TeamDetailsModel> GetTeamByCodeAsync(string teamCode)
        {
            try
            {
                var result = await _teamRepository.GetTeamByCode(teamCode);
                if (result == null)
                {
                    throw new ArgumentException($"No Team Found with teamCode '{teamCode}'");
                } 
                var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
                if (currentSeason == null)
                {
                    throw new ArgumentException($"No data Found for current season");
                }
                var driverSeasonMappings = await _seasonRepository.GetDriversForTeamAsync(result.TeamId, currentSeason.SeasonId);
                var teamStandings = await _raceStandingsRepository.GetTeamStandingsByTeamIdAsync(result.TeamId);
                var currentDrivers = new List<string>();
                foreach (var mapping in driverSeasonMappings)
                {
                    var driver = await _driverRepository.GetDriverById(mapping.DriverId);
                    if (driver != null)
                    {
                        currentDrivers.Add($"{driver.FirstName} {driver.LastName}");
                    }
                }
                return new TeamDetailsModel
                {
                    TeamCode = teamCode,
                    Name = result.TeamName,
                    Nationality = result.Nationality,
                    Url = result.Url,
                    CurrentDrivers = currentDrivers
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<TeamDetailsModel>> GetTeamsByCurrentSeasonAsync()
        {
            try
            {
                var currentSeason = await _seasonRepository.GetCurrentSeasonAsync();
                if (currentSeason == null)
                {
                    throw new ArgumentException($"No data Found for current season");
                }
                var result = await _seasonRepository.TeamDriversBySeasonAsync(currentSeason.SeasonId);
                var teams = new List<TeamDetailsModel>();
                foreach (var team in result)
                {
                    var teamDetails = await _teamRepository.GetTeamById(team.TeamId);
                    var currentDrivers = new List<string>();
                    var driverSeasonMappings = await _seasonRepository.GetDriversForTeamAsync(team.TeamId, currentSeason.SeasonId);
                    foreach (var mapping in driverSeasonMappings)
                    {
                        var driver = await _driverRepository.GetDriverById(mapping.DriverId);
                        if (driver != null)
                        {
                            currentDrivers.Add($"{driver.FirstName} {driver.LastName}");
                        }
                    }

                    var response = new TeamDetailsModel
                    {
                        TeamCode = teamDetails.TeamCode,
                        Name = teamDetails.TeamName,
                        Nationality = teamDetails.Nationality,
                        Url = teamDetails.Url,
                        CurrentDrivers = currentDrivers
                    };
                    teams.Add(response);
                }
                return teams;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TeamModel> AddTeamAsync(TeamModel model)
        {
            try
            {
                var existingDriver = await _teamRepository.GetTeamByCode(model.TeamCode);
                if (existingDriver != null)
                {
                    throw new ArgumentException($"Team with code '{model.TeamCode}' already exists.");
                }
                var team = new Team
                {
                    TeamCode = model.TeamCode,
                    TeamName = model.TeamName,
                    Nationality = model.Nationality,
                    Url = model.Url
                };
                var result = await _teamRepository.AddTeam(team);
                var response = new TeamModel
                {
                    TeamCode = model.TeamCode,
                    TeamName = model.TeamName,
                    Nationality = model.Nationality,
                    Url = model.Url
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
