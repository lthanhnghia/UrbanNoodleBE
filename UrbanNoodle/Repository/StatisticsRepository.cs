using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Repository.Interface;

namespace UrbanNoodle.Repository
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StatisticsRepository> _logger;

        public StatisticsRepository(ApplicationDbContext context, ILogger<StatisticsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardSummaryDto> GetDashboardAsync()
        {
            var sql = @"
                    with order_summary as (
                    select sum(total) as TotalRevenue, count(*) as TotalOrders
                    from orders
            ),
                    food_by_quantity as (
                    select fd.food_name, sum(oi.quantity) as total_quantity
                    from order_items oi
                    join food fd on oi.food_id = fd.id
                    group by fd.food_name
                    order by total_quantity desc
                    limit 1
            ),
                    food_by_orders as (
                    select fd.food_name, count(distinct oi.order_id) as order_count
                    from order_items oi
                    join food fd on oi.food_id = fd.id
                    group by fd.food_name
                    order by order_count desc
                    limit 1
            )
                    select
                    os.TotalRevenue,
                    os.TotalOrders,
                    fq.food_name as MostOrderedByQuantity,
                    fq.total_quantity as TotalQuantity,
                    fo.food_name as MostFrequentInOrders,
                    fo.order_count as OrderCount
                    from order_summary os, food_by_quantity fq, food_by_orders fo
            ";
            var result = await _context.Database
                .SqlQueryRaw<DashboardSummaryDto>(sql)
                .ToListAsync();
            var summary = result.FirstOrDefault();
            return summary;
        }
    }
}
