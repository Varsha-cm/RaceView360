using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBSeasonRepository
    {
        Task<Season> AddSeasonAsync(Season season);
        Task<Season> GetSeasonIdByYearAsync(int Year);
        Task<List<Season>> GetAllSeasonsAsync();
        Task<Season> GetSeasonIdByIdAsync(int seasonId);
        Task AddDriverTeamAsync(DriverSeasonMapping driverTeamSeason);
        Task UpdateDriverTeamAsync(DriverSeasonMapping driverTeamSeason);
        Task<DriverSeasonMapping> IsDriverMappedToSeasonAsync(int driverId, int seasonId);
        Task<DriverSeasonMapping> IsDriverMappedToTeamAsync(int driverId, int teamId, int seasonId);
        Task<List<DriverSeasonMapping>> GetDriversForTeamAsync(int teamId, int seasonId);
        Task<List<DriverSeasonMapping>> TeamDriversBySeasonAsync(int seasonId);
        Task<int> GetDriverCountForTeamAsync(int teamId,int seasonId);
        Task<Season> GetCurrentSeasonAsync();
    }
}
