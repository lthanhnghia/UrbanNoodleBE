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
                _logger.LogError(ex, "Lỗi tạo đơn hàng cho account {AccountId} và xảy ra tại {Message}", request.AccountId, ex.Message);
                await transaction.RollbackAsync();
                return new ApiResponse(500, "Lỗi hệ thống");
            }
        }

        public async Task<IEnumerable<GetOrderDto>> GetOrderAsync(int lastId, string? statusName, int size)
        {
            var result = await _context.Order
                     .OrderByDescending(od => od.Id)
                     .Where(od => (statusName == null || od.Status.StatusName == statusName) && (lastId == 0 || od.Id < lastId))
                     .Take(size)
                     .Select(od => new GetOrderDto
                     {
                         ClientName = od.OrderedByUser.FullName,
                         ClientPhone = od.OrderedByUser.Phone,
                         ClientAddress = od.Address.DetailAddress,
                         OrderId = od.Id,
                         StatusName = od.Status.StatusName,
                         CreatedAt = od.CreatedAt,
                         TotalPrice = od.Total,
                         Items = od.OrdersItems.Select(oi => new OrderItemFoodDto
                         {
                             FoodName = oi.Food.FoodName,
                             Quantity = oi.Quantity,
                             Price = oi.Price
                         }).ToList()
                     })
                     .ToListAsync();
            return result;
        }

        public async Task<ApiResponse> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto request)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                throw new NotFoundException("Không tìm thấy đơn hàng này");
            }

            var currentStatus = await _context.Status.FindAsync(order.StatusId);
            if (currentStatus == null)
            {
                throw new NotFoundException("Trạng thái hiện tại của đơn hàng không hợp lệ");
            }


            if (currentStatus.StatusName != request.StatusName)
            {
                throw new BadRequestException(
                    $"Trạng thái đơn hàng đã thay đổi (hiện tại: {currentStatus.StatusName}), vui lòng tải lại trang.");
            }

            if (currentStatus.StatusName == "success" || currentStatus.StatusName == "cancelled")
            {
                throw new BadRequestException("Không thể chuyển đơn này sang trạng thái khác được nữa");
            }

            string? nextStatusName = currentStatus.StatusName switch
            {
                "ordered" => "confirmed",
                "confirmed" => "success",
                _ => null
            };

            if (nextStatusName == null)
            {
                throw new BadRequestException("Trạng thái đơn hàng không hợp lệ để chuyển tiếp");
            }

            var newStatus = await _context.Status
                .FirstOrDefaultAsync(s => s.StatusName == nextStatusName);
            if (newStatus == null)
            {
                throw new NotFoundException("Không có trạng thái này trong hệ thống");
            }

            order.StatusId = newStatus.Id;
            await _context.SaveChangesAsync();

            return new ApiResponse(200, "Cập nhật trạng thái đơn hàng thành công");
        }
    }
}
