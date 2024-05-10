using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment.Infrastructure.Repository.RaceRepository
{
    public class CircuitRepository : IDBCircuitRepository
    {
        private readonly RaceViewContext _context;
        public CircuitRepository(RaceViewContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Circuit> AddCircuit(Circuit circuit)
        {
            _context.Circuits.Add(circuit);
            await _context.SaveChangesAsync();
            return circuit;
        }


        public async Task<Circuit> GetByCodeAsync(string circuitCode)
        {
            return await _context.Circuits.FirstOrDefaultAsync(c => c.CircuitCode == circuitCode);
        }
        public async Task<Circuit> GetCircuitByIdAsync(int circuitId)
        {
            return await _context.Circuits.FirstOrDefaultAsync(c => c.CircuitId == circuitId);
        }

        public async Task<IEnumerable<Circuit>> GetAllAsync()
        {
            return await _context.Circuits.ToListAsync();
        }

        public async Task UpdateAsync(Circuit circuit)
        {
            _context.Entry(circuit).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit != null)
            {
                _context.Circuits.Remove(circuit);
                await _context.SaveChangesAsync();
            }
        }

    }
}
