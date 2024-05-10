using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBLapsRepository
    {
        Task<LapTime> AddLiveRace(LapTime lapTime);
        Task<List<LapTime>> GetLapsForRaceAndRoundType(int raceId, RoundType roundType);

    }
}
