using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Services;
using UrbanNoodle.Services.Interface;

namespace UrbanNoodle.Controllers
{
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
        public async Task<DashboardDto> GetAccount(
     [FromQuery] DateTime? start = null,
     [FromQuery] DateTime? end = null)
        {

            return await _service.GetDashboard(start,end);
        }
    }
}
