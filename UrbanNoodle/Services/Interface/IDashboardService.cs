using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface IDashboardService
    {
        public Task<DashboardSummaryDto> GetDashboardStatisticsAsync();
    }
}
