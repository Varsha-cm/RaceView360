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
    public class StandingService
    {
        private readonly IDBRaceRepository _raceRepository;
        private readonly IDBDriverRepository _driverRepository;
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBLapsRepository _lapRepository;
        private readonly IDBRoundRepository _roundRepository;
        private readonly IDBRaceResultRepository _raceResultRepository;
        private readonly IDBTeamRepository _teamRepository;
        private readonly IDBRaceStandingsRepository _raceStandingsRepository;

        public StandingService(IDBRaceStandingsRepository dBRaceStandingsRepository, IDBTeamRepository dBTeamRepository, IDBRoundRepository dBRoundRepository, IDBRaceResultRepository dBRaceResultRepository, IDBRaceRepository dBRaceRepository, IDBLapsRepository dBLapRepository, IDBDriverRepository dBDriverRepository, IDBSeasonRepository dbSeasonRepository)
        {
            _raceRepository = dBRaceRepository;
            _driverRepository = dBDriverRepository;
            _seasonRepository = dbSeasonRepository;
            _lapRepository = dBLapRepository;
            _roundRepository = dBRoundRepository;
            _raceResultRepository = dBRaceResultRepository;
            _teamRepository = dBTeamRepository;
            _raceStandingsRepository = dBRaceStandingsRepository;
        }

        public async Task<List<SeasonDriverStandingModel>> GetDriverStandingsForSeason(int year)
        {
            var season = await _seasonRepository.GetSeasonIdByYearAsync(year);
            if (season == null)
            {
                throw new ArgumentException($"Season {year} not found.");
            }
            var races = await _raceRepository.GetAllRacesForSeason(season.SeasonId);
            var raceIds = races.Select(r => r.RaceId).ToList();
            var driverStandings = await _raceStandingsRepository.GetDriverStandingsForYear(season.SeasonId, raceIds);
            var driverPoints = new Dictionary<int, int>(); 
            foreach (var driverStanding in driverStandings)
            {
                if (!driverPoints.ContainsKey(driverStanding.DriverId))
                {
                    driverPoints[driverStanding.DriverId] = 0;
                }
                driverPoints[driverStanding.DriverId] += (int) driverStanding.Points;
            }

            var sortedDriverIds = driverPoints.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
            var position = 1;
            var seasonDriverStandings = new List<SeasonDriverStandingModel>();

            foreach (var driverId in sortedDriverIds)
            {
                var driver = await _driverRepository.GetDriverById(driverId);
                var totalPoints = driverPoints[driverId];
                var teamName = driverStandings.FirstOrDefault(ds => ds.DriverId == driverId)?.Team?.TeamName ?? "Unknown";

                var model = new SeasonDriverStandingModel
                {
                    Position = position++,
                    DriverName = $"{driver.FirstName} {driver.LastName}",
                    Nationality = driver.Nationality,
                    Team = teamName,
                    Points = totalPoints,
                    
                };
                seasonDriverStandings.Add(model);
            }

            return seasonDriverStandings;
        }


        public async Task<List<DriverStandingModel>> GetDriverStandingsForRaces(int year, string driverCode)
        {
            var season = await _seasonRepository.GetSeasonIdByYearAsync(year);
            if (season == null)
            {
                throw new ArgumentException($"Season {year} not found.");
            }
            var driver = await _driverRepository.GetDriverByCode(driverCode);
            if (driver == null)
            {
                throw new ArgumentException($"Driver with driverCode {driverCode} not found.");
            }
            var driverStandings = await _raceStandingsRepository.GetDriverStandingsForRaces(season.SeasonId, driver.DriverId);
            if (driverStandings == null || !driverStandings.Any())
            {
                throw new ArgumentException("Driver standings not found.");
            }
            var response = driverStandings.Select(ds => new DriverStandingModel
            {
                GrandPrix = ds.Race.RaceName,
                Date = ds.Race.RaceDateTime,
                Car = ds.Team.TeamName,
                RacePosition = ds.Position,
                Points = ds.Points
            }).ToList();

            return response;
        }

        public async Task<List<SeasonTeamStandingModel>> GetTeamStandingsForSeason(int year)
        {
            var season = await _seasonRepository.GetSeasonIdByYearAsync(year);
            if (season == null)
            {
                throw new ArgumentException($"Season {year} not found.");
            }
            var races = await _raceRepository.GetAllRacesForSeason(season.SeasonId);
            if (races == null)
            {
                throw new ArgumentException("Data not found");
            }
            var raceIds = races.Select(r => r.RaceId).ToList();
            var teamStandings = await _raceStandingsRepository.GetTeamStandingsForYear(season.SeasonId, raceIds);
            if (teamStandings.Count() == 0)
            {
                throw new ArgumentException($"Data not found");
            }
            var teamPoints = new Dictionary<int, int>();
            foreach (var teamStanding in teamStandings)
            {
                if (!teamPoints.ContainsKey(teamStanding.TeamId))
                {
                    teamPoints[teamStanding.TeamId] = 0;
                }
                teamPoints[teamStanding.TeamId] += (int)teamStanding.Points;
            }

            var sortedTeamIds = teamPoints.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
            var position = 1;
            var seasonTeamStandings = new List<SeasonTeamStandingModel>();

            foreach (var teamId in sortedTeamIds)
            {
                var team = await _teamRepository.GetTeamById(teamId);
                var totalPoints = teamPoints[teamId];

                var model = new SeasonTeamStandingModel
                {
                    Position = position++,
                    TeamName = team.TeamName,
                    Points = totalPoints
                };

                seasonTeamStandings.Add(model);
            }

            return seasonTeamStandings;
        }

        public async Task<List<TeamStandingModel>> GetTeamStandingsForRaces(int year, string teamCode)
        {
            var season = await _seasonRepository.GetSeasonIdByYearAsync(year);
            if (season == null)
            {
                throw new ArgumentException($"Season {year} not found.");
            }
            var team = await _teamRepository.GetTeamByCode(teamCode);
            if (team == null)
            {
                throw new ArgumentException($"Team with code {teamCode} not found.");
            }
            var teamStandings = await _raceStandingsRepository.GetTeamStandingsForRaces(season.SeasonId, team.TeamId);
            if (teamStandings == null || !teamStandings.Any())
            {
                throw new ArgumentException("Team standings not found.");
            }

            var response = teamStandings.Select(ts => new TeamStandingModel
            {
                GrandPrix = ts.Race.RaceName,
                Date = ts.Race.RaceDateTime,
                Points = ts.Points
            }).ToList();

            return response;
        }

    }
}
