using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanNoodle.Dto;
using UrbanNoodle.Services.Interface;

namespace UrbanNoodle.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [EnableRateLimiting("AdminPolicy")]
        public async Task<DashboardSummaryDto> GetAccount()
        {

            return await _service.GetDashboardStatisticsAsync();
        }
    }
}
