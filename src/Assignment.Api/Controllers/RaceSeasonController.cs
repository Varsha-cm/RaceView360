using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("season")]
    [ApiController]
    public class RaceSeasonController : ControllerBase
    {
        private readonly SeasonService _seasonService;

        public RaceSeasonController(SeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        //[CustomAuthorize("race-manage")]
        [HttpPost]
        public async Task<IActionResult> AddSeason([FromBody] SeasonModel model)
        {
            try
            {
                var createdSeason = await _seasonService.AddSeasonAsync(model);
                return Ok(createdSeason);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while adding the season: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSeasons()
        {
            try
            {
                var seasons = await _seasonService.GetAllSeasonAsync();
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = seasons });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching data: {ex.Message}" });
            }
        }

        [HttpGet("{year}")]
        public async Task<IActionResult> GetSeason(int year)
        {
            try
            {
                var season = await _seasonService.GetSeasonByYearAsync(year);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = season });
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

        [CustomAuthorize("race-manage")]
        [HttpPost("driverTeam/mapping")]
        public async Task<IActionResult> MapDriverTeam([FromBody] DriverTeamMappingModel model)
        {
            try
            {
                await _seasonService.MapDriverTeamAsync(model);
                return Ok(new { StatusCode = 200, Message = "Mapped Successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while adding the driver-team-season mapping: {ex.Message}" });
            }
        }

        [CustomAuthorize("race-manage")]
        [HttpPost("driverTeam/unmapping")]
        public async Task<IActionResult> UnmapDriverTeam([FromBody] DriverTeamMappingModel model)
        {
            try
            {
                await _seasonService.UnmapDriverTeamAsync(model);
                return Ok(new { StatusCode = 200, Message = "Unmapped Successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while adding the driver-team-season mapping: {ex.Message}" });
            }
        }

    }
}
