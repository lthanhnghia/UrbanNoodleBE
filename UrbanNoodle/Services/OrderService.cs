using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Order;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order()
                {
                    DiningtablesId = request.DiningTableId,
                    AccountsId = request.AccountId,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow,
                };
                _context.Order.Add(order);
                await _context.SaveChangesAsync();

              
                var orderItem = new List<OrdersItem>();
                decimal total = 0; 
                foreach (var item in request.Item) {
                    var food = await _context.Food.FindAsync(item.FoodId);
                    if(food == null)
                    {
                        throw new NotFoundException("Không có món ăn này trong quán");
                    }
                    orderItem.Add(new OrdersItem
                    {
                        OrdersId = order.Id,
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Price = food.Price,
                        CreatedAt = DateTime.UtcNow
                    });
                    _logger.LogInformation("Thông tin: "+order.Id + " - " + item.FoodId);
                    _logger.LogInformation(item.Quantity+" - "+food.Price);
                   
                    total += item.Quantity * food.Price;
                }
                _context.OrderItems.AddRange(orderItem);
                await _context.SaveChangesAsync();
                order.Total = total;
               
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse(200, "Tạo đơn hàng thành công");
            }
            catch (Exception ex) {
                _logger.LogInformation("Lỗi: " + ex.Message);
                await transaction.RollbackAsync();
                return new ApiResponse(500, "Lỗi hệ thống");
            }
        }
    }
}
