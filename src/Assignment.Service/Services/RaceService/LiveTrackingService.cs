using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Service.Services.RaceService
{
    public class LiveTrackingService
    {
        private readonly IDBRaceRepository _raceRepository;
        private readonly IDBDriverRepository _driverRepository;
        private readonly IDBSeasonRepository _seasonRepository;
        private readonly IDBLapsRepository _lapRepository;
        private readonly IDBRoundRepository _roundRepository;
        private readonly IDBRaceResultRepository _raceResultRepository;
        private readonly IDBTeamRepository _teamRepository;

        public LiveTrackingService(IDBTeamRepository dBTeamRepository, IDBRoundRepository dBRoundRepository, IDBRaceResultRepository dBRaceResultRepository, IDBRaceRepository dBRaceRepository, IDBLapsRepository dBLapRepository, IDBDriverRepository dBDriverRepository, IDBSeasonRepository dbSeasonRepository)
        {
            _raceRepository = dBRaceRepository;
            _driverRepository = dBDriverRepository;
            _seasonRepository = dbSeasonRepository;
            _lapRepository = dBLapRepository;
            _roundRepository = dBRoundRepository;
            _raceResultRepository = dBRaceResultRepository;
            _teamRepository = dBTeamRepository;
        }

        public async Task InsertLapDetailsAsync(string raceCode, UpdateLiveRaceRQ lapDetails)
        {
            var raceDetails = await _raceRepository.GetRaceByCodeAsync(raceCode);
            if (raceDetails == null)
            {
                throw new ArgumentException($"No Race Found with raceCode '{raceCode}'");
            }
            var rounds = await _roundRepository.GetRoundsByRaceId(raceDetails.RaceId);
            if (rounds == null)
            {
                throw new ArgumentException($"No Round Found for race '{raceCode}'");
            }
            Round currentRound = null;
            foreach (var round in rounds)
            {
                if (round.RoundStatus == Status.Inprogress)
                {
                    currentRound = round;
                    break;
                }
            }
            if (currentRound == null)
            {
                throw new ArgumentException($"Cannot insert lap details. The race '{raceCode}' is not in progress.");
            }
            var driverDetails = await _driverRepository.GetDriverByNumber(lapDetails.CarNumber);
            if (driverDetails == null)
            {
                throw new ArgumentException($"No Driver Found with car number '{lapDetails.CarNumber}'");
            }
            var isDriverMappedToSeason = await _seasonRepository.IsDriverMappedToSeasonAsync(driverDetails.DriverId, raceDetails.SeasonId);
            if (isDriverMappedToSeason == null)
            {
                throw new ArgumentException($"Driver with car number '{lapDetails.CarNumber}' is not participating in the current season ");
            }

            var seasonDrivers = await _seasonRepository.TeamDriversBySeasonAsync(raceDetails.SeasonId);


            List<int> driversToInclude = new List<int>();
            switch (currentRound.RoundType)
            {

                case RoundType.Q2:
                    var q1Results = await _raceResultRepository.GetQualifyingResultAsync(raceDetails.RaceId);
                    driversToInclude = q1Results.OrderBy(qr => qr.Position).Take(15).Select(qr => qr.DriverId).ToList();
                    break;
                case RoundType.Q3:
                    var q2Results = await _raceResultRepository.GetQualifyingResultAsync(raceDetails.RaceId);
                    driversToInclude = q2Results.OrderBy(qr => qr.Position).Take(10).Select(qr => qr.DriverId).ToList();
                    break;
                default:
                    driversToInclude = seasonDrivers.Select(driver => driver.DriverId).ToList();
                    break;
            }


            if (!driversToInclude.Contains(driverDetails.DriverId))
            {
                throw new ArgumentException($"Driver with car number '{lapDetails.CarNumber}' is not qualified for this round.");
            }


            int lapNumber = await GetNextLapNumberAsync(raceDetails.RaceId, driverDetails.DriverId, currentRound.RoundType);
            TimeOnly timeOnly = lapDetails.LapTime;
            var lap = new LapTime
            {
                RaceId = raceDetails.RaceId,
                RoundId = currentRound.RoundId,
                RoundType = currentRound.RoundType,
                DriverId = driverDetails.DriverId,
                Lap = lapNumber,
                LapTime1 = timeOnly.ToTimeSpan(),
        };
            await _lapRepository.AddLiveRace(lap);
        }

        public async Task StartRound(string raceCode, RoundType roundType)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No Race Found with raceCode '{raceCode}'");
            }
            var allRaces = await _raceRepository.GetAllRacesForSeason(race.SeasonId);
            var inProgressRaces = allRaces.Where(r => r.Status == Status.Inprogress).ToList();
            if (inProgressRaces.Any() && inProgressRaces.First().RaceCode != race.RaceCode)
            {
                throw new ArgumentException($"Cannot start a new race while another race '{inProgressRaces.First().RaceCode}' is already in progress.");
            }
            var rounds = await _roundRepository.GetRoundsByRaceId(race.RaceId);
            if (rounds.Any())
            {
                foreach (var round in rounds)
                {
                    if (round.RoundStatus == Status.Inprogress)
                    {
                        throw new ArgumentException($"Cannot start another round '{roundType}' for race '{raceCode}' while '{round.RoundType}' is already in progress.");
                    }
                    if (round.RoundType == roundType && round.RoundStatus == Status.Completed)
                    {
                        throw new ArgumentException($"The round type '{roundType}' for race '{raceCode}' has already been completed.");
                    }
                }
            }
            race.Status = Status.Inprogress;
            var roundDetails = new Round()
            {
                RaceId = race.RaceId,
                RoundType = roundType,
                RoundStatus = Status.Inprogress,
                StartTime  = DateTime.Now
            };
            var raceDetails = await _raceRepository.UpdateRace(race);
            await _roundRepository.AddRoundDetails(roundDetails);
        }

        public async Task<Race> EndRound(string raceCode)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No Race Found with raceCode '{raceCode}'");
            }
            var rounds = await _roundRepository.GetRoundsByRaceId(race.RaceId);
            Round currentRound = null;
            foreach (var round in rounds)
            {
                if (round.RoundStatus == Status.Inprogress)
                {
                    currentRound = round;
                    break;
                }
            }
            if (currentRound == null)
            {
                throw new ArgumentException($"No round is currently in progress for race '{raceCode}'");
            }

            if (currentRound.RoundType == RoundType.Race)
            {
                race.Status = Status.Completed;
                await UpdateRaceResults(raceCode, currentRound.RoundType);
            }
            else if (currentRound.RoundType == RoundType.Practice1 || currentRound.RoundType == RoundType.Practice2 || currentRound.RoundType == RoundType.Practice3)
            {
                await UpdatePracticeResults(raceCode, currentRound.RoundType);
            }
            else if (currentRound.RoundType == RoundType.Q1 || currentRound.RoundType == RoundType.Q2 || currentRound.RoundType == RoundType.Q3)
            {
                await UpdateQualifyingResults(raceCode, currentRound.RoundType);
            }
            currentRound.RoundStatus = Status.Completed;
            currentRound.EndTime = DateTime.UtcNow;
            var raceDetails = await _raceRepository.UpdateRace(race);
            var roundDetails = await _roundRepository.UpdateRoundDetails(currentRound);
            return raceDetails;
        }


        public async Task<object> GetCurrentRaceStatusAsync(string raceCode)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            if (race == null)
            {
                throw new ArgumentException($"No Race Found with raceCode '{raceCode}'");
            }
            var rounds = await _roundRepository.GetRoundsByRaceId(race.RaceId);
            var currentRound = rounds.FirstOrDefault(round => round.RoundStatus == Status.Inprogress);
            if (currentRound == null)
            {
                throw new ArgumentException($"No round is currently in progress for race '{raceCode}'");
            }

            var lapTimes = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, currentRound.RoundType);
            var totalLapsByDriver = lapTimes.GroupBy(l => l.DriverId)
                                            .Select(g => new { DriverID = g.Key, TotalLapsCompleted = g.Max(l => l.Lap), FastestLapTime = g.Min(l => l.LapTime1) })
                                            .ToList();
            var fastestLapTimesByDriver = lapTimes.GroupBy(l => l.DriverId)
                                                  .Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) })
                                                  .ToList();
            var seasonDrivers = await _seasonRepository.TeamDriversBySeasonAsync(race.SeasonId);

            var sortedDrivers = currentRound.RoundType == RoundType.Race ?
                totalLapsByDriver.OrderByDescending(d => d.TotalLapsCompleted)
                                 .Select(d => new DriverLapInfo { DriverID = d.DriverID, TotalLapsCompleted = d.TotalLapsCompleted, FastestLapTime = d.FastestLapTime })
                                 .ToList() :
                fastestLapTimesByDriver.OrderBy(d => d.FastestLapTime)
                                       .Select(d => new DriverLapInfo { DriverID = d.DriverID, FastestLapTime = d.FastestLapTime })
                                       .ToList();

            var practiceResults = new List<PracticeResult>();
            List<int> driversToInclude = new List<int>();
            switch (currentRound.RoundType)
            {
                case RoundType.Q2:
                    var q1Results = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
                    driversToInclude = q1Results.OrderBy(x =>x.Position).Take(15).Select(qr => qr.DriverId).ToList();
                    break;
                case RoundType.Q3:
                    var q2Results = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
                    driversToInclude = q2Results.OrderBy(x => x.Position).Take(10).Select(qr => qr.DriverId).ToList();
                    break;
                default:
                    driversToInclude = seasonDrivers.Select(driver => driver.DriverId).ToList();
                    break;
            }
            int lastPosition = driversToInclude.Count;

            foreach (var driverId in driversToInclude)
            {
                var driver = seasonDrivers.FirstOrDefault(d => d.DriverId == driverId);
                var driverLapTime = sortedDrivers.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                var position = 0;

                if (totalLapsByDriver.Any(d => d.DriverID == driverId))
                {
                    var driverPosition = sortedDrivers.FindIndex(d => d.DriverID == driverId) + 1;
                    position = driverPosition;
                }
                else
                {
                    position = lastPosition;
                    lastPosition--;
                }

                var numberOfLaps = lapTimes.Count(l => l.DriverId == driverId);
                var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId, race.SeasonId);
                var practiceResult = new PracticeResult
                {
                    RaceId = race.RaceId,
                    DriverId = driverId,
                    TeamId = teamDriver.TeamId,
                    PracticeType = currentRound.RoundType.ToString(),
                    FastestLapTime = driverLapTime,
                    Position = position,
                    LapsCompleted = numberOfLaps
                };

                practiceResults.Add(practiceResult);
            }

            practiceResults = practiceResults.OrderBy(pr => pr.Position).ToList();

            var raceStatus = new List<object>();

            foreach (var practiceResult in practiceResults)
            {
                var driverDetails = await _driverRepository.GetDriverById(practiceResult.DriverId);
                var teamDetails = await _teamRepository.GetTeamById(practiceResult.TeamId);

                raceStatus.Add(new
                {
                    position = practiceResult.Position,
                    carNumber = driverDetails.DriverNumber,
                    driverName = $"{driverDetails.FirstName} {driverDetails.LastName}",
                    teamName = teamDetails.TeamName,
                    fastestLap = practiceResult.FastestLapTime,
                    lapsCompleted = practiceResult.LapsCompleted
                });
            }

            return new
            {
                raceName = race.RaceName,
                roundType = currentRound.RoundType,
                raceStatus = raceStatus
            };
        }


        public async Task UpdateRaceResults(string raceCode, RoundType roundType)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            var lapTimes = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, roundType);
            var totalLapsByDriver = lapTimes.GroupBy(l => l.DriverId).ToDictionary(g => g.Key, g => g.Max(l => l.Lap));
            var sortedDrivers = totalLapsByDriver.OrderByDescending(d => d.Value).ToList();
            var fastestLapTimesByDriver = lapTimes.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
            var seasonDrivers = await _seasonRepository.TeamDriversBySeasonAsync(race.SeasonId);
            var qualifyingResults = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
            var gridPositions = qualifyingResults.OrderBy(qr => qr.Position).Take(20).Select(qr => qr.DriverId).ToList();
            int lastPosition = seasonDrivers.Count;
            var teamPoints = new Dictionary<int, int>();
            var raceResults = new List<RaceResult>();
            var driverStandings = new List<DriverStanding>();
            var teamStandings = new List<TeamStanding>();
            var driverWithFastestLap = fastestLapTimesByDriver.OrderBy(x => x.FastestLapTime).FirstOrDefault();
            foreach (var driver in seasonDrivers)
            {
                var driverId = driver.DriverId;
                var totalLaps = sortedDrivers.FirstOrDefault(d => d.Key == driverId).Value;
                var driverFastestLapTime = fastestLapTimesByDriver.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                var qualifyingResult = await _raceResultRepository.GetQualifyingResultForDriversAsync(driver.DriverId, race.RaceId, RoundType.Q3);
                var gridPosition = gridPositions.IndexOf(driverId) + 1;
                var finishingTime = lapTimes.Where(l => l.DriverId == driver.DriverId).Select(l => l.LapTime1).Aggregate(TimeSpan.Zero, (acc, cur) => acc + cur);
                var position = 0;
                if (totalLapsByDriver.ContainsKey(driverId))
                {
                    var driverPosition = sortedDrivers.FindIndex(d => d.Key == driverId) + 1;
                    position = driverPosition;
                }
                else
                {
                    position = lastPosition;
                    lastPosition--;
                }
                var numberOfLaps = lapTimes.Count(l => l.DriverId == driverId);
                var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId,race.SeasonId);
                var points = CalculatePointsEarned(position);
                
                var result = new RaceResult
                {
                    RaceId = race.RaceId,
                    DriverId = driverId,
                    TeamId = teamDriver.TeamId,
                    FastestLapTime = driverFastestLapTime,
                    Position = position,
                    LapsCompleted = numberOfLaps,
                    GridPosition = gridPosition,
                    FinishingTime = finishingTime,
                    PointsEarned = points,
                };
                raceResults.Add(result);
                var driverStanding = new DriverStanding
                {
                    DriverId = driver.DriverId,
                    RaceId = race.RaceId,
                    Position = position,
                    Points = points,
                    TeamId = teamDriver.TeamId,
                };
                driverStandings.Add(driverStanding);
                if (!teamPoints.ContainsKey(teamDriver.TeamId))
                {
                    teamPoints[teamDriver.TeamId] = 0;
                }
                teamPoints[teamDriver.TeamId] += points;

                if (driver.DriverId == driverWithFastestLap?.DriverID)
                {
                    driverStanding.Points++;
                    var teamStanding = teamStandings.FirstOrDefault(ts => ts.TeamId == teamDriver.TeamId);
                    if (teamStanding != null)
                    {
                        teamStanding.Points++;
                    }
                }
            }

            if (driverWithFastestLap != null)
            {
                var driverIdWithFastestLap = driverWithFastestLap.DriverID;
                var raceResultWithFastestLap = raceResults.FirstOrDefault(r => r.DriverId == driverIdWithFastestLap);
                if (raceResultWithFastestLap != null)
                {
                    raceResultWithFastestLap.PointsEarned += 1;
                }
            }
            var teamPointsList = teamPoints.Select(tp => new { TeamId = tp.Key, Points = tp.Value }).ToList();
            var sortedTeams = teamPointsList.OrderByDescending(tp => tp.Points).ToList();
            int teamPosition = 1;

            foreach (var teamPoint in sortedTeams)
            {
                var teamStanding = new TeamStanding
                {
                    RaceId = race.RaceId,
                    TeamId = teamPoint.TeamId,
                    Points = teamPoint.Points,
                    Position =  teamPosition
                };

                teamStandings.Add(teamStanding);
                teamPosition++;
            }
            driverStandings = driverStandings.OrderBy(pr => pr.Position).ToList();
            raceResults = raceResults.OrderBy(pr => pr.Position).ToList();
            await _raceResultRepository.UpdateRaceResultAsync(raceResults);
            await _raceResultRepository.UpdateDriverStanding(driverStandings);
            await _raceResultRepository.UpdateTeamStanding(teamStandings);
        }

        public async Task UpdatePracticeResults(string raceCode, RoundType roundType)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            var lapTimes = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, roundType);
            var fastestLapTimesByDriver = lapTimes.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
            var sortedDrivers = fastestLapTimesByDriver.OrderBy(d => d.FastestLapTime).ToList();
            var totalLapsByDriver = lapTimes.GroupBy(l => l.DriverId).ToDictionary(g => g.Key, g => g.Max(l => l.Lap));
            var seasonDrivers = await _seasonRepository.TeamDriversBySeasonAsync(race.SeasonId);
            var practiceResults = new List<PracticeResult>();
            int lastPosition = seasonDrivers.Count;
            foreach (var driver in seasonDrivers)
            {
                var driverId = driver.DriverId;
                var driverLapTime = sortedDrivers.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                var position = 0;
                if (totalLapsByDriver.ContainsKey(driverId))
                {
                    var driverPosition = sortedDrivers.FindIndex(d => d.DriverID == driverId) + 1;
                    position = driverPosition;
                }
                else
                {
                    position = lastPosition;
                    lastPosition--;
                }

                var numberOfLaps = lapTimes.Count(l => l.DriverId == driverId);
                var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId, race.SeasonId);
                var practiceResult = new PracticeResult
                {
                    RaceId = race.RaceId,
                    DriverId = driverId,
                    TeamId = teamDriver.TeamId,
                    PracticeType = roundType.ToString(),
                    FastestLapTime = driverLapTime,
                    Position = position,
                    LapsCompleted = numberOfLaps
                };

                practiceResults.Add(practiceResult);
            }
            practiceResults = practiceResults.OrderBy(pr => pr.Position).ToList();
            await _raceResultRepository.UpdatePracticeResultsAsync(practiceResults);
        }

        public async Task<int> GetNextLapNumberAsync(int raceId, int driverId, RoundType roundType)
        {
            var lapTimes = await _lapRepository.GetLapsForRaceAndRoundType(raceId, roundType);
            var driverLapTimes = lapTimes.Where(l => l.DriverId == driverId);
            int latestLapNumber = driverLapTimes.Any() ? driverLapTimes.Max(l => l.Lap) : 0;
            int nextLapNumber = latestLapNumber + 1;
            return nextLapNumber;
        }
        public static int CalculatePointsEarned(int position)
        {
            var pointsByPosition = new Dictionary<int, int>
            {
                { 1, 25 },
                { 2, 18 },
                { 3, 15 },
                { 4, 12 },
                { 5, 10 },
                { 6, 8 },
                { 7, 6 },
                { 8, 4 },
                { 9, 2 },
                { 10, 1 }
            };

            return pointsByPosition.ContainsKey(position) ? pointsByPosition[position] : 0;
        }


        public async Task UpdateQualifyingResults(string raceCode, RoundType roundType)
        {
            var race = await _raceRepository.GetRaceByCodeAsync(raceCode);
            var lapTimes = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, roundType);
            var seasonDrivers = await _seasonRepository.TeamDriversBySeasonAsync(race.SeasonId);
            var qualifyingResults = new List<QualifyingResult>();
            var totalLapsByDriver = lapTimes.GroupBy(l => l.DriverId).ToDictionary(g => g.Key, g => g.Max(l => l.Lap));

            switch (roundType)
            {
                case RoundType.Q1:
                    var fastestLapTimesQ1 = lapTimes.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
                    var sortedDriversQ1 = fastestLapTimesQ1.OrderBy(d => d.FastestLapTime).ToList();
                    int lastPosition = 20;
                    foreach (var driverId in seasonDrivers.Select(driver => driver.DriverId))
                    {
                        var driver = seasonDrivers.FirstOrDefault(d => d.DriverId == driverId);
                        var driverLapTime = sortedDriversQ1.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                        var numberOfLapsQ1 = lapTimes.Count(l => l.DriverId == driverId);
                        int position = 0;
                        if (totalLapsByDriver.ContainsKey(driverId))
                        {
                            var driverPosition = sortedDriversQ1.FindIndex(d => d.DriverID == driverId) + 1;
                            position = driverPosition;
                        }
                        else
                        {
                            position = lastPosition;
                            lastPosition--;
                        }
                        var numberOfLaps = lapTimes.Count(l => l.DriverId == driverId);
                        var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId, race.SeasonId);
                        var qualifyingResult = new QualifyingResult
                        {
                            RaceId = race.RaceId,
                            DriverId = driverId,
                            TeamId = teamDriver.TeamId,
                            Q1 = driverLapTime,
                            Position = position,
                            LapsCompleted = numberOfLapsQ1
                        };
                        qualifyingResults.Add(qualifyingResult);
                    }
                    break;

                case RoundType.Q2:

                    var previousQualifyingResult = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
                    var top15DriversFromQ1 = previousQualifyingResult.OrderBy(qr => qr.Position).Take(15).Select(qr => qr.DriverId).ToList();
                    fastestLapTimesQ1 = (await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, RoundType.Q1))
                         .GroupBy(l => l.DriverId)
                         .Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) })
                         .ToList();
                    sortedDriversQ1 = fastestLapTimesQ1.OrderBy(d => d.FastestLapTime).ToList();
                    var fastestLapTimesQ2 = lapTimes.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
                    var sortedDriversQ2 = fastestLapTimesQ2.OrderBy(d => d.FastestLapTime).ToList();
                    lastPosition = 15;
                    foreach (var driverId in top15DriversFromQ1)
                    {
                        var driver = seasonDrivers.FirstOrDefault(d => d.DriverId == driverId);
                        var lapsCompletedQ1 = previousQualifyingResult.FirstOrDefault(qr => qr.DriverId == driverId)?.LapsCompleted ?? 0;
                        var numberOfLapsQ2 = lapTimes.Count(l => l.DriverId == driverId) ;
                        numberOfLapsQ2 += lapsCompletedQ1;
                        int positionQ2 = totalLapsByDriver.ContainsKey(driverId) ? sortedDriversQ2.FindIndex(d => d.DriverID == driverId) + 1 : lastPosition--;
                        var driverLapTimeQ1 = sortedDriversQ1.FirstOrDefault(qr => qr.DriverID == driverId)?.FastestLapTime;
                        var driverLapTimeQ2 = sortedDriversQ2.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                        var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId, race.SeasonId);
                        var qualifyingResult = new QualifyingResult
                        {
                            RaceId = race.RaceId,
                            DriverId = driverId,
                            TeamId = teamDriver.TeamId,
                            Q1 = driverLapTimeQ1,
                            Q2 = driverLapTimeQ2,
                            Position = positionQ2,
                            LapsCompleted = numberOfLapsQ2
                        };
                        var existingResultQ1 = qualifyingResults.FirstOrDefault(qr => qr.DriverId == driverId && qr.RaceId == race.RaceId);
                        if (existingResultQ1 != null)
                        {
                            existingResultQ1.Q2 = driverLapTimeQ2;
                            existingResultQ1.Position = positionQ2;
                           
                        }
                        else
                        {
                            qualifyingResults.Add(qualifyingResult);
                        }
                    }
                    break;

                case RoundType.Q3:
                    previousQualifyingResult = await _raceResultRepository.GetQualifyingResultAsync(race.RaceId);
                    var top10DriversFromQ2 = previousQualifyingResult.OrderBy(qr => qr.Position).Take(10).Select(qr => qr.DriverId).ToList();
                    fastestLapTimesQ1 = (await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, RoundType.Q1))
                         .GroupBy(l => l.DriverId)
                         .Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) })
                         .ToList();
                    sortedDriversQ1 = fastestLapTimesQ1.OrderBy(d => d.FastestLapTime).ToList();
                    fastestLapTimesQ2 = (await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, RoundType.Q2))
                         .GroupBy(l => l.DriverId)
                         .Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) })
                         .ToList();
                    sortedDriversQ2 = fastestLapTimesQ2.OrderBy(d => d.FastestLapTime).ToList();
                    var fastestLapTimesQ3 = lapTimes.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
                    var sortedDriversQ3 = fastestLapTimesQ3.OrderBy(d => d.FastestLapTime).ToList();
                    lastPosition = 11  ;
                    foreach (var driverId in top10DriversFromQ2)
                    {
                        var driver = seasonDrivers.FirstOrDefault(d => d.DriverId == driverId);
                        var q1Details = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, RoundType.Q1);
                        var lapsCompletedQ2 = previousQualifyingResult.FirstOrDefault(qr => qr.DriverId == driverId)?.LapsCompleted ?? 0;
                        fastestLapTimesQ1 = q1Details.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
                        sortedDriversQ1 = fastestLapTimesQ1.OrderBy(d => d.FastestLapTime).ToList();
                        var driverLapTimeQ1 = sortedDriversQ1.FirstOrDefault(qr => qr.DriverID == driverId)?.FastestLapTime;

                        int positionQ3 = totalLapsByDriver.ContainsKey(driverId) ? sortedDriversQ3.FindIndex(d => d.DriverID == driverId) + 1 : --lastPosition;

                        var q2Details = await _lapRepository.GetLapsForRaceAndRoundType(race.RaceId, RoundType.Q2);
                        fastestLapTimesQ2 = q2Details.GroupBy(l => l.DriverId).Select(g => new { DriverID = g.Key, FastestLapTime = g.Min(l => l.LapTime1) }).ToList();
                        sortedDriversQ2 = fastestLapTimesQ2.OrderBy(d => d.FastestLapTime).ToList();
                        var driverLapTimeQ2 = sortedDriversQ2.FirstOrDefault(qr => qr.DriverID == driverId)?.FastestLapTime;

                        var driverLapTimeQ3 = sortedDriversQ3.FirstOrDefault(d => d.DriverID == driverId)?.FastestLapTime;
                        var numberOfLapsQ3 = lapTimes.Count(l => l.DriverId == driverId);
                        numberOfLapsQ3 += lapsCompletedQ2;
                        var teamDriver = await _seasonRepository.IsDriverMappedToSeasonAsync(driverId, race.SeasonId    );
                        var qualifyingResult = new QualifyingResult
                        {
                            RaceId = race.RaceId,
                            DriverId = driverId,
                            TeamId = teamDriver.TeamId,
                            Q1 = driverLapTimeQ1,
                            Q2 = driverLapTimeQ2,
                            Q3 = driverLapTimeQ3,
                            Position = positionQ3,
                            LapsCompleted = numberOfLapsQ3 
                        };
                        
                            qualifyingResults.Add(qualifyingResult);
                        
                    }
                    break;
            }

            qualifyingResults = qualifyingResults.OrderBy(pr => pr.Position).ToList();
            await _raceResultRepository.UpdateQualifyingResultAsync(qualifyingResults);

        }


    }
}
