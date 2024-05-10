using Assignment.Api.Models;
using Assignment.Infrastructure.Repository.RaceRepository;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class RaceResultController : ControllerBase
    {
        private readonly LiveTrackingService _liveTrackingService;
        private readonly RaceService _raceService;
        private readonly SeasonService _seasonService;
        private readonly RaceResultService _raceResultService;

        public RaceResultController(RaceResultService raceResultService,SeasonService seasonService,RaceService raceService ,LiveTrackingService liveTrackingService)
        {
            _liveTrackingService = liveTrackingService;
            _raceService = raceService;
            _seasonService = seasonService;
            _raceResultService = raceResultService;
        }



        [HttpGet("season/{year}/races/results")]
        public async Task<IActionResult> GetRaceResultsForSeason(int year)
        {
            var races = await _raceResultService.GetRaceResultsForRaces(year);
            if (races == null || !races.Any())
            {
                return NotFound("No races found for the specified season.");
            }
            return Ok(races);
        }

        [HttpGet("season/{year}/race/{raceCode}/race/result")]
        public async Task<IActionResult> GetRaceResults(int year, string raceCode)
        {
            try
            {
                var teams = await _raceResultService.GetRaceResultsAsync(year, raceCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = teams });
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

        [HttpGet("season/{year}/race/{raceCode}/{practice}/result")]
        public async Task<IActionResult> GetPracticeResults(int year, string raceCode, Practice practice)
        {
            try
            {
                var teams = await _raceResultService.GetPracticeResultsAsync(year, raceCode, practice);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = teams });
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

        [HttpGet("season/{year}/race/{raceCode}/qualify/results")]
        public async Task<IActionResult> GetQualifyingResults(int year, string raceCode)
        {
            try
            {
                var teams = await _raceResultService.GetQualifyingResultsAsync(year, raceCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = teams });
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

    }
}
