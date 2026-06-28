using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Dto.Address;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto.Order;
using UrbanNoodle.Entities;

namespace UrbanNoodle.Services
{
    public class ToolAlService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ToolAlService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? _currentAccountId;

        public void SetCurrentUser(int? accountId)
        {
            _currentAccountId = accountId;
            _logger.LogInformation("SetCurrentUser called - accountId: " + accountId + " | HashCode: " + this.GetHashCode());

        }

        public ToolAlService()
        {
        }

        public ToolAlService(ApplicationDbContext context, ILogger<ToolAlService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [KernelFunction("get_food")]
        [Description("Lấy danh sách món ăn, giá, category. Chỉ gọi khi lịch sử chat chưa có menu.")]
        public async Task<IEnumerable<GetFoodDto>> GetFood()
        {

            var query = await _context.Food.OrderBy(fd => fd.Id)
                .Where(fd => fd.IsDeleted == false)
                 .Select(fd => new GetFoodDto
                 {
                     Id = fd.Id,
                     Name = fd.FoodName,
                     Price = fd.Price,
                     CategoryName = fd.Category.CategoryName
                 }).ToListAsync();
            return query;
        }

        [KernelFunction("create_order")]
        [Description(
    "Tạo đơn hàng. Chỉ gọi khi:\n" +
    "1. Khách đã đăng nhập\n" +
    "2. Đã hiển thị tóm tắt đơn hàng\n" +
    "3. Khách xác nhận bằng 'Đồng ý'/'Xác nhận'/'Approve'\n" +
    "4. Dùng address_id nếu chọn địa chỉ cũ, new_address nếu địa chỉ mới."
)]
        public async Task<ApiResponse> CreateOrderAsync(CreateOrderDto request)
        {

            _logger.LogInformation("ToolService - this HashCode: " + this.GetHashCode());
            _logger.LogInformation("ToolService - _currentAccountId: " + _currentAccountId);
            if (_currentAccountId == null)
                return new ApiResponse(401, "Chưa đăng nhập");


            if (request.Item == null || !request.Item.Any())
                return new ApiResponse(400, "Đơn hàng không có món ăn");

            request.AccountId = _currentAccountId.Value;
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



        [KernelFunction("get_customer_profile")]
        [Description(
     "Lấy họ tên, SĐT và danh sách địa chỉ đã lưu của khách.\n" +
     "Chỉ gọi khi khách đã đăng nhập. Chỉ gọi 1 lần duy nhất."
 )]
        public async Task<GetAccountDTO?> GetAccountLookupListAsync()
        {


            if (_currentAccountId == null)
                return null;


            var account = await _context.Account
                     .Where(a => a.Id == _currentAccountId.Value)
                     .Select(a => new GetAccountDTO
                     {
                         Id = a.Id,
                         Fullname = a.FullName,
                         Phone = a.Phone,

                         Addresses = a.Addresses
                     .Select(ad => new AddressByAccount
                     {
                         Id = ad.Id,
                         Name = ad.DetailAddress
                     })
                        .ToList()
                     })
                     .FirstOrDefaultAsync();
            return account;
        }
    }
}
