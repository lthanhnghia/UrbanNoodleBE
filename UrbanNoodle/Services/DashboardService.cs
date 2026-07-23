using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Repository.Interface;
using UrbanNoodle.Services.Interface;

namespace UrbanNoodle.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;
        private readonly IStatisticsRepository _statisticsRepository;

        public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger, IStatisticsRepository statisticsRepository)
        {
            _context = context;
            _logger = logger;
            _statisticsRepository = statisticsRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardStatisticsAsync()
        {
            var result = await _statisticsRepository.GetDashboardAsync();
            return result;
        }
    }
}
