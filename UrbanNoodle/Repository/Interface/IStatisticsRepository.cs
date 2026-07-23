using UrbanNoodle.Dto;

namespace UrbanNoodle.Repository.Interface
{
    public interface IStatisticsRepository
    {
        public Task<DashboardSummaryDto> GetDashboardAsync();
    }
}
