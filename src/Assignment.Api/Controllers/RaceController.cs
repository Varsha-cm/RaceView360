using Assignment.Api.Models;
using Assignment.Infrastructure.Repository.RaceRepository;
using Assignment.Service.Model;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class RaceController : ControllerBase
    {
        private readonly RaceService _raceService;
        private readonly AuthService _authService;

        public RaceController(RaceService raceService, AuthService authService)
        {
            _raceService = raceService;
            _authService = authService;
        }

        [CustomAuthorize("race-manage")]
        [HttpPost("race")]
        public async Task<IActionResult> AddRace([FromBody] RaceModel model)
        {
            try
            {
                var createdRace = await _raceService.AddRaceAsync(model);
                return CreatedAtAction(nameof(AddRace), new { driverCode = createdRace.RaceCode }, new { StatusCode = 201, Message = "Created successfully", Response = createdRace });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while adding the race: {ex.Message}" });
            }
        }

        [CustomAuthorize("race-manage")]
        [HttpPut("race/{raceCode}")]
        public async Task<IActionResult> UpdateRace(string raceCode, [FromBody] UpdateRaceModel model)
        {
            try
            {
                var createdRace = await _raceService.UpdateRaceAsync(raceCode,model);
                return Ok(new { StatusCode = 200, Message = "Updated successfully", Response = createdRace });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while adding the race: {ex.Message}" });
            }
        }

        [HttpGet("races/season/{year}")]
        public async Task<IActionResult> GetRacesBySeasonId(int year)
        {
            try
            {
                var races = await _raceService.GetAllRacesForSeason(year);
                if (races == null || !races.Any())
                {
                    return NotFound();
                }
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = races });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching data: {ex.Message}" });
            }
        }


        [HttpGet("race/{raceCode}")]
        public async Task<IActionResult> GetRaceById(string raceCode)
        {
            try
            {
                var race = await _raceService.GetRaceForSeason(raceCode);
                if (race == null)
                {
                    return NotFound();
                }
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = race });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching data: {ex.Message}" });
            }
        }

        [HttpGet("race/previous")]
        public async Task<ActionResult<RaceModel>> GetPreviousRace()
        {
            try
            {
                var previousRace = await _raceService.GetPreviousRaceAsync();
                return Ok(previousRace);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { StatusCode = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("race/inprogress")]
        public async Task<ActionResult<RaceModel>> GetInprogressRace()
        {
            try
            {
                var nextRace = await _raceService.GetRaceInprogressAsync();
                return Ok(nextRace);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { StatusCode = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("races/upcoming")]
        public async Task<ActionResult<List<RaceModel>>> GetUpcomingRaces()
        {
            try
            {
                var upcomingRaces = await _raceService.GetUpcomingRacesAsync(2);
                return Ok(upcomingRaces);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { StatusCode = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
