using Assignment.Api.Interfaces.RaceInterface;
using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit = Assignment.Api.Models.Circuit;

namespace Assignment.Service.Services.RaceService
{
    public class CircuitService
    {
        private readonly IDBCircuitRepository _circuitRepository;

        public CircuitService(IDBCircuitRepository dBCircuitRepository) : base()
        {
            _circuitRepository = dBCircuitRepository;
        }

        public async Task<CircuitModel> AddCircuitAsync(CircuitModel circuitRequest)
        {
            try
            {
                var circuitExist = await _circuitRepository.GetByCodeAsync(circuitRequest.CircuitCode);
                if (circuitExist != null)
                {
                    throw new ArgumentException($"Circuit with code '{circuitRequest.CircuitCode}' already exists.");
                }
                var circuit = new Circuit()
                {
                    CircuitCode = circuitRequest.CircuitCode,
                    Name = circuitRequest.CircuitName,
                    Location = circuitRequest.Location,
                    Country = circuitRequest.Country,
                    Length = circuitRequest.CircuitLength,
                    Laps = circuitRequest.Laps,
                    RaceDistance = circuitRequest.RaceDistance,
                    Url = circuitRequest.Url,
                };
                var result = await _circuitRepository.AddCircuit(circuit);
                var circuitResponse = new CircuitModel()
                {
                    CircuitCode = circuit.CircuitCode,
                    CircuitName = circuit.Name,
                    Location = circuit.Location,
                    Country = circuit.Country,
                    CircuitLength = circuitRequest.CircuitLength,
                    Laps = circuitRequest.Laps,
                    RaceDistance = circuitRequest.RaceDistance,
                    Url = circuitRequest.Url,
                };
                return circuitResponse;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }


        public async Task<CircuitModel> UpdateCircuitAsync(string circuitCode,UpdateCircuitRQ circuitRequest)
        {
            try
            {
                var existingCircuit = await _circuitRepository.GetByCodeAsync(circuitCode);
                if (existingCircuit == null)
                {
                    throw new ArgumentException($"Circuit with code '{circuitCode}' doesn't exists.");
                }
                existingCircuit.Name = circuitRequest.CircuitName;
                existingCircuit.RaceDistance = circuitRequest.RaceDistance;
                existingCircuit.Laps = circuitRequest.Laps;
                existingCircuit.Length = circuitRequest.CircuitLength;
                existingCircuit.Url = circuitRequest.Url;
                await _circuitRepository.UpdateAsync(existingCircuit);
                var circuitResponse = new CircuitModel()
                {
                    CircuitCode = existingCircuit.CircuitCode,
                    CircuitName = existingCircuit.Name,
                    Location = existingCircuit.Location,
                    Country = existingCircuit.Country,
                    CircuitLength = circuitRequest.CircuitLength,
                    Laps = circuitRequest.Laps,
                    RaceDistance = circuitRequest.RaceDistance,
                    Url = circuitRequest.Url,
                };
                return circuitResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CircuitModel>> GetAllAsync()
        {
            var circuits = await _circuitRepository.GetAllAsync();
            return circuits.Select(MapToModel);
        }

        public async Task<CircuitModel> GetByCodeAsync(string circuitCode)
        {
            var circuit = await _circuitRepository.GetByCodeAsync(circuitCode);
            if (circuit == null)
            {
                throw new ArgumentException($"Circuit with code {circuitCode} not found");
            }
            return MapToModel(circuit);
        }

        private CircuitModel MapToModel(Circuit circuit)
        {
            return new CircuitModel
            {
                CircuitCode = circuit.CircuitCode,
                CircuitName = circuit.Name,
                Location = circuit.Location,
                Country = circuit.Country,
                CircuitLength = circuit.Length,
                Laps = circuit.Laps,
                RaceDistance = circuit.RaceDistance,
                Url = circuit.Url
            };


        }
    }
}
