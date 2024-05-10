using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("team")]
    [ApiController]
    public class RaceTeamController : ControllerBase
    {
        private readonly TeamService _teamService;

        public RaceTeamController(TeamService teamService)
        {
            _teamService = teamService;
        }

        [CustomAuthorize("race-manage")]
        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamModel model)
        {
            try
            {
                var createdTeam = await _teamService.AddTeamAsync(model);
                return CreatedAtAction(nameof(GetTeamByCode), new { teamCode = createdTeam.TeamCode }, new
                {
                    StatusCode = 201,
                    Message = "Created successfully",
                    Response = createdTeam
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"An error occurred while creating driver: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                var teams = await _teamService.GetAllTeamAsync();
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = teams });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching data: {ex.Message}" });
            }
        }


        [HttpGet("currentSeason")]
        public async Task<ActionResult<List<TeamModel>>> GetCurrentSeasonTeams()
        {
            try
            {
                var teams = await _teamService.GetTeamsByCurrentSeasonAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{teamCode}")]
        public async Task<IActionResult> GetTeamByCode(string teamCode)
        {
            try
            {
                var team = await _teamService.GetTeamByCodeAsync(teamCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = team });
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
