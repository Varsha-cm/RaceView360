using Assignment.Infrastructure.Repository.RaceRepository;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class RaceStandingsController : ControllerBase
    {
        private readonly LiveTrackingService _liveTrackingService;
        private readonly RaceService _raceService;
        private readonly SeasonService _seasonService;
        private readonly RaceResultService _raceResultService;
        private readonly StandingService _standingService;

        public RaceStandingsController(StandingService standingService,RaceResultService raceResultService, SeasonService seasonService, RaceService raceService, LiveTrackingService liveTrackingService)
        {
            _liveTrackingService = liveTrackingService;
            _raceService = raceService;
            _seasonService = seasonService;
            _raceResultService = raceResultService;
            _standingService = standingService;
        }

        [HttpGet("season/{year}/driver/{driverCode}/driverStandings")]
        public async Task<ActionResult<List<DriverStandingModel>>> GetDriverStandingsForRaces(int year, string driverCode)
        {
            try
            {
                var driverStandings = await _standingService.GetDriverStandingsForRaces(year,driverCode);
                return Ok(driverStandings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("season/{year}/driverStandings")]
        public async Task<ActionResult<List<SeasonDriverStandingModel>>> GetDriverStandingsForSeason(int year)
        {
            try
            {
                var driverStandings = await _standingService.GetDriverStandingsForSeason(year);
                return Ok(driverStandings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving driver standings for season {year}: {ex.Message}");
            }
        }

        [HttpGet("season/{year}/teamStandings")]
        public async Task<IActionResult> GetTeamStandingsForSeason(int year)
        {
            try
            {
                var standings = await _standingService.GetTeamStandingsForSeason(year);
                return Ok(standings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("season/{year}/team/{teamCode}/teamStandings")]
        public async Task<IActionResult> GetTeamStandingsForRaces(int year, string teamCode)
        {
            try
            {
                var standings = await _standingService.GetTeamStandingsForRaces(year, teamCode);
                return Ok(standings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


    }
}
