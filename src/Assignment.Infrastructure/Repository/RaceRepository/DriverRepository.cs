using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repository.RaceRepository
{
    public class DriverRepository : IDBDriverRepository
    {
        private readonly RaceViewContext _context;
        public DriverRepository(RaceViewContext dbContext) 
        {
            _context = dbContext;
        }
        public async Task<Driver> AddDriver(Driver driver)
        {
            await _context.AddAsync(driver);
            _context.SaveChanges();
            return driver;
        }
        public async Task<List<Driver>> GetAllDrivers()
        {
            return await _context.Drivers.ToListAsync();
        }
        public async Task<Driver> GetDriverByCode(string driverCode)
        {
            var result = await _context.Drivers.FirstOrDefaultAsync(x => x.DriverCode == driverCode);
            return result;
        }

        public async Task<Driver> GetDriverById(int driverId)
        {
            var result = await _context.Drivers.FirstOrDefaultAsync(x => x.DriverId == driverId);
            return result;
        }

        public async Task<Driver> GetDriverByNumber(int driverNumber)
        {
            var result = await _context.Drivers.FirstOrDefaultAsync(x => x.DriverNumber == driverNumber);
            return result;
        }

        public async Task<bool> IsDriverNumberExistsAsync(int driverNumber)
        {
            return await _context.Drivers.AnyAsync(d => d.DriverNumber == driverNumber);
        }

        public async Task<bool> IsDriverNameExistsAsync(string firstName, string lastName)
        {
            return await _context.Drivers.AnyAsync(d => d.FirstName == firstName && d.LastName == lastName);
        }

    }
}
