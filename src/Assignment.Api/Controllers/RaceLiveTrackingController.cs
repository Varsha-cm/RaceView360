using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("race")]
    [ApiController]
    public class RaceLiveTrackingController : ControllerBase
    {
        private readonly LiveTrackingService _liveTrackingService;

        public  RaceLiveTrackingController(LiveTrackingService liveTrackingService)
        {
            _liveTrackingService = liveTrackingService;
        }
        [CustomAuthorize("race-manage")]
        [HttpPut("{raceCode}/{roundType}/startRace")]
        public async Task<IActionResult> StartRound(string raceCode, RoundType roundType)
        {
            try
            {
                await _liveTrackingService.StartRound(raceCode, roundType);
                return Ok(new { StatusCode = 200, Message = "Round started successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while starting round: {ex.Message}" });
            }
        }

        [CustomAuthorize("race-manage")]
        [HttpPut("{raceCode}/endRace")]
        public async Task<IActionResult> EndRound(string raceCode)
        {
            try
            {
                var result = await _liveTrackingService.EndRound(raceCode);
                return Ok(new { StatusCode = 200, Message = "Round ended successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while ending round: {ex.Message}" });
            }

        }

        [CustomAuthorize("race-manage")]
        [HttpPost("{raceCode}/laptimeDetails/update")]
        public async Task<IActionResult> InsertLapDetails(string raceCode, [FromBody] UpdateLiveRaceRQ lapDetails)
        {
            try
            {
                await _liveTrackingService.InsertLapDetailsAsync(raceCode, lapDetails);
                return Ok(new { StatusCode = 200, Message = "Updated Successfully"});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while inserting lap details: {ex.Message}" });
            }
        }

        [HttpGet("{raceCode}/liveTracking")]
        public async Task<IActionResult> GetCurrentRaceStatus(string raceCode)
        {
            try
            {
                var raceStatus = await _liveTrackingService.GetCurrentRaceStatusAsync(raceCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", RaceStatus = raceStatus });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching race status: {ex.Message}" });
            }
        }
    
}
}
