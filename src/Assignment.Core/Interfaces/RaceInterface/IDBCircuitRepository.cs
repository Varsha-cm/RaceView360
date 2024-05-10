using Assignment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Api.Interfaces.RaceInterface
{
    public interface IDBCircuitRepository
    {
        Task<Circuit> AddCircuit(Circuit circuit);
        Task<Circuit> GetByCodeAsync(string circuitCode);
        Task<Circuit> GetCircuitByIdAsync(int circuitId);
        Task<IEnumerable<Circuit>> GetAllAsync();
        Task UpdateAsync(Circuit circuit);
        Task DeleteAsync(int id);
    }
}
