using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBRoundRepository
    {
        Task<List<Round>> GetRoundsByRaceId(int raceId);
        Task<Round> AddRoundDetails(Round round);
        Task<Round> UpdateRoundDetails(Round round);
    }
}
