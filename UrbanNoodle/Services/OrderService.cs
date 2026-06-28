using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Order;
using UrbanNoodle.Entities;
using UrbanNoodle.Services.Interface;

namespace UrbanNoodle.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;
        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiResponse> CreateOrderAsync(CreateOrderDto request)
        {
            // Validate trước khi mở transaction
            if (request.Item == null || !request.Item.Any())
                return new ApiResponse(400, "Đơn hàng không có món ăn");

            var foodIds = request.Item.Select(i => i.FoodId).ToList();
            var foods = await _context.Food
                .Where(f => foodIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id);

            var missingFoodId = foodIds.FirstOrDefault(id => !foods.ContainsKey(id));
            if (missingFoodId != default)
                return new ApiResponse(404, "Không có món ăn này trong quán");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    AddressId = request.AddressId,
                    OrderedUser = request.AccountId,
                    StatusId = 1,
                    CreatedAt = DateTime.UtcNow,
                };
                _context.Order.Add(order);
                await _context.SaveChangesAsync(); // lần 1: lấy order.Id

                decimal total = 0;
                var orderItems = request.Item.Select(item =>
                {
                    var food = foods[item.FoodId];
                    total += item.Quantity * food.Price;
                    return new OrdersItem
                    {
                        OrderId = order.Id,
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Price = food.Price,
                        CreatedAt = DateTime.UtcNow
                    };
                }).ToList();

                order.Total = total;
                _context.OrderItems.AddRange(orderItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new ApiResponse(200, "Tạo đơn hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo đơn hàng cho account {AccountId}", request.AccountId);
                await transaction.RollbackAsync();
                return new ApiResponse(500, "Lỗi hệ thống");
            }
        }
    }
}
