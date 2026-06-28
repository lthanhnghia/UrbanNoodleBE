using UrbanNoodle.ApplicationContext;

namespace UrbanNoodle.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //public async Task<DashboardDto> GetDashboard(DateTime? start, DateTime? end)
        //{
        //    DateTime fromDate;
        //    DateTime toDate;

        //    if (start.HasValue && end.HasValue)
        //    {
        //        fromDate = DateTime.SpecifyKind(start.Value.Date, DateTimeKind.Utc);
        //        toDate = DateTime.SpecifyKind(end.Value.Date.AddDays(1), DateTimeKind.Utc);
        //    }
        //    else
        //    {
        //        fromDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        //        toDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
        //    }

        //    var result = await _context.Order
        //                .Where(ord => ord.CreatedAt >= fromDate && ord.CreatedAt< toDate)
        //                .GroupBy(x => 1)
        //                .Select(g => new
        //                {
        //                    TotalAmount = g.Sum(x => x.Total),
        //                    NumberCount = g.Count()
        //                }).FirstOrDefaultAsync();

        //    var numberAccount = await _context.Account.CountAsync(x => x.IsDeleted==false);

        //    var topFood = await _context.OrderItems
        //         .Where(x => x.Orders.CreatedAt >= fromDate && x.Orders.CreatedAt < toDate)
        //                 .GroupBy(x => new { x.FoodId, x.Food.Name })
        //                 .Select(g => new
        //                 {
        //                     FoodName = g.Key.Name,
        //                     TotalQuantity = g.Sum(x => x.Quantity)
        //                 }).OrderByDescending(x => x.TotalQuantity)
        //                 .FirstOrDefaultAsync();

        //    var foodRaw = await _context.OrderItems
        //         .Where(x => x.Orders.CreatedAt >= fromDate && x.Orders.CreatedAt < toDate)
        //        .GroupBy(x => new { x.FoodId, x.Food.Name })
        //        .Select(g => new
        //        {
        //            FoodName = g.Key.Name,
        //            OrderNumber = g.Count(),
        //            Revenue = g.Sum(x => x.Price * x.Quantity)
        //        }).OrderByDescending(x => x.Revenue)
        //        .Take(3).ToListAsync();

        //    var foodTop = foodRaw
        //        .Select(x => new TopFoodDto(
        //               x.FoodName,
        //               x.OrderNumber,
        //               x.Revenue
        //             )).ToList();
        //        return new DashboardDto(
        //            result?.TotalAmount ?? 0,
        //            result?.NumberCount ?? 0,
        //            topFood?.FoodName ?? "Không có dữ liệu",
        //            topFood?.TotalQuantity ?? 0,
        //            numberAccount,
        //            foodTop
        //        );
        //}
    }
}
