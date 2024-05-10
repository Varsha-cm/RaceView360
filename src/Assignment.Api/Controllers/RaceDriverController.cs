using Assignment.Api.Models;
using Assignment.Service.Model.RaceViewModels;
using Assignment.Service.Services.RaceService;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("race/driver")]
    [ApiController]
    public class RaceDriverController : ControllerBase
    {
        private readonly DriverService _driverService;
        public RaceDriverController(DriverService driverService)
        {
            _driverService = driverService;
        }


        [CustomAuthorize("race-manage")]
        [HttpPost]
        public async Task<IActionResult> CreateDriver(DriverModel model)
        {
            try
            {
                var createdDriver = await _driverService.AddDriverAsync(model);
                return CreatedAtAction(nameof(GetDriver), new { driverCode = createdDriver.DriverCode }, new
                {
                    StatusCode = 201,
                    Message = "Created successfully",
                    Response = createdDriver
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { StatusCode = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while creating driver: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDrivers()
        {
            try
            {
                var drivers = await _driverService.GetAllDriversAsync();
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = drivers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while fetching data: {ex.Message}" });
            }
        }

        [HttpGet("season/{year}")]
        public async Task<IActionResult> GetDriversBySeason(int season)
        {
            try
            {
                var drivers = await _driverService.GetDriversBySeason(season);
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{driverCode}")]
        public async Task<IActionResult> GetDriver(string driverCode)
        {
            try
            {
                var driver = await _driverService.GetDriverByCodeAsync(driverCode);
                return Ok(new { StatusCode = 200, Message = "Fetched Successfully", response = driver });
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

