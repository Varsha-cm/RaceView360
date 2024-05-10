using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("circuit")]
    [ApiController]
    public class RaceCircuitController : BaseController
    {
        public readonly CircuitService _circuitService;
        public RaceCircuitController(CircuitService circuitService, Serilog.Core.Logger logger) : base(logger)
        {
            _circuitService = circuitService;
        }

        [CustomAuthorize("race-manage")]
        [HttpPost]
        public async Task<IActionResult> CreateCircuit([FromBody] CircuitModel circuit)
        {
            try
            {
                var createdCircuit = await _circuitService.AddCircuitAsync(circuit);
                return Ok(new { StatusCode = 200, Message = "Circuit added successfully", response = createdCircuit });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Bad request", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error", Error = $"An error occurred while creating driver: {ex.Message}" });

            }
        }
        
        [CustomAuthorize("race-manage")]
        [HttpPut("circuitCode")]
        public async Task<IActionResult> UpdateCircuit(string circuitCode,UpdateCircuitRQ circuit)
        {
            try
            {
                var createdCircuit = await _circuitService.UpdateCircuitAsync(circuitCode,circuit);
                return Ok(new { StatusCode = 200, Message = "Circuit updated successfully", response = createdCircuit });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = "Bad request", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error", Error = $"An error occurred while creating driver: {ex.Message}" });

            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CircuitModel>>> GetAllCircuits()
        {
            try
            {
                var circuits = await _circuitService.GetAllAsync();
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = circuits });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching circuits: {ex.Message}");
            }
        }

        [HttpGet("{circuitCode}")]
        public async Task<ActionResult<CircuitModel>> GetCircuitByCode(string circuitCode)
        {
            try
            {
                var circuit = await _circuitService.GetByCodeAsync(circuitCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = circuit });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching circuit: {ex.Message}");
            }
        }

    }
}
