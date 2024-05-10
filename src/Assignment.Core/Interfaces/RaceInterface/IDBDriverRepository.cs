using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{   
    public interface IDBDriverRepository
    {
        Task<Driver> AddDriver(Driver driver);
        Task<List<Driver>> GetAllDrivers();
        Task<Driver> GetDriverByCode(string driverCode);
        Task<Driver> GetDriverById(int driverId);
        Task<Driver> GetDriverByNumber(int driverNumber);
        Task<bool> IsDriverNameExistsAsync(string firstName, string lastName);
        Task<bool> IsDriverNumberExistsAsync(int driverNumber);


    }
}
