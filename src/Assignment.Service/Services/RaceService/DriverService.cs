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
    public class DriverService
    {
        private readonly IDBDriverRepository _driverRepository;
        private readonly IDBRaceStandingsRepository _raceStandingsRepository;
        private readonly IDBSeasonRepository _seasonRepository;

        public DriverService(IDBSeasonRepository dBSeasonRepository,IDBRaceStandingsRepository dBRaceStandingsRepository,IDBDriverRepository dBDriverRepository)
        {
            _driverRepository = dBDriverRepository;
            _raceStandingsRepository = dBRaceStandingsRepository;
            _seasonRepository = dBSeasonRepository;
        }


        public async Task<List<DriverModel>> GetAllDriversAsync()
        {
            try
            {
                var result = await _driverRepository.GetAllDrivers();
                var drivers = new List<DriverModel>();
                foreach (var driver in result)
                {

                    var response = new DriverModel
                    {
                        FirstName = driver.FirstName,
                        LastName = driver.LastName,
                        DriverCode = driver.DriverCode,
                        DriverNumber = driver.DriverNumber,
                        Nationality = driver.Nationality,
                        DateOfBirth = driver.DateOfBirth,
                        Url = driver.Url,
                    };
                    drivers.Add(response);
                }
                return drivers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DriverModel>> GetDriversBySeason(int season)
        {
            try
            {
                var seasonDetails = await _seasonRepository.GetSeasonIdByYearAsync(season);
                if (seasonDetails == null)
                {
                    throw new ArgumentException($"No data Found for the season {season}");
                }
                var result = await _seasonRepository.TeamDriversBySeasonAsync(seasonDetails.SeasonId);
                var drivers = new List<DriverModel>();
                foreach (var driver in result)
                {
                    var driverDetails = await _driverRepository.GetDriverById(driver.DriverId);
                    var response = new DriverModel
                    {
                        FirstName = driverDetails.FirstName,
                        LastName = driverDetails.LastName,
                        DriverCode = driverDetails.DriverCode,
                        DriverNumber = driverDetails.DriverNumber,
                        Nationality = driverDetails.Nationality,
                        DateOfBirth = driverDetails.DateOfBirth,
                        Url = driverDetails.Url,
                    };
                    drivers.Add(response);
                }
                return drivers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DriverDetailsModel> GetDriverByCodeAsync(string driverCode)
        {
            try
            {
                var result = await _driverRepository.GetDriverByCode(driverCode);
                if (result == null)
                {
                    throw new ArgumentException($"No Drivers Found with driverCode '{driverCode}'");
                }
                var driverStandings = await _raceStandingsRepository.GetDriverStandingsByDriverIdAsync(result.DriverId);
                var totalPoints = driverStandings.Sum(ds => ds.Points);
                var racesParticipated = driverStandings.Count;
                return new DriverDetailsModel
                {
                    DriverCode = result.DriverCode,
                    DriverNumber = result.DriverNumber,
                    Name = result.FirstName + " " + result.LastName,
                    Nationality = result.Nationality,
                    TotalPoints = (int)totalPoints,
                    RacesParticipated = racesParticipated,
                    DateOfBirth = result.DateOfBirth,
                };
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<DriverModel> AddDriverAsync(DriverModel model)
        {
            try
            {
                var existingDriver = await _driverRepository.GetDriverByCode(model.DriverCode);
                if (existingDriver != null)
                {
                    throw new ArgumentException($"Driver with code '{model.DriverCode}' already exists.");
                }
                var driverName = await _driverRepository.IsDriverNameExistsAsync(model.FirstName, model.LastName);
                if(driverName)
                {
                    throw new ArgumentException($"Driver with name '{model.FirstName},{model.LastName}' already exists.");
                }
                var driverNumber = await _driverRepository.IsDriverNumberExistsAsync((int)model.DriverNumber);
                if (driverNumber)
                {
                    throw new ArgumentException($"Driver number '{model.DriverNumber}' has already been taken by another driver.");
                }

                var driver = new Driver
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DriverCode = model.DriverCode,
                    DriverNumber = model.DriverNumber,
                    Nationality = model.Nationality,
                    DateOfBirth = model.DateOfBirth,
                    Url = model.Url,    
                };
                var result = await _driverRepository.AddDriver(driver);
                var response = new DriverModel
                {
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    DriverCode = result.DriverCode,
                    DriverNumber = result.DriverNumber,
                    Nationality = result.Nationality,
                    DateOfBirth = result.DateOfBirth,
                    Url = result.Url,
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
