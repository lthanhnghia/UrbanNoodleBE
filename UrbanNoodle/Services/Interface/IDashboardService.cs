using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface IDashboardService
    {
        public Task<DashboardDto> GetDashboard(DateTime? start,DateTime? end);
    }
}
