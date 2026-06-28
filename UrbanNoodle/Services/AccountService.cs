using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Service
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountService> _logger;
        public AccountService(ApplicationDbContext context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiResponse> CreateAccountAsync(AccountDTO request)
        {
            //if (await _context.Account.AnyAsync(u => u.Phone.Equals(request.Phone)))
            //{
            //    throw new BadRequestException("Số điện thoại này đã được sử dụng");
            //}
            //string SearchName = UtilService.NormalizeText(request.FullName);
            //var account = new Account(request.FullName, SearchName, request.Phone, 1, DateTime.UtcNow);
            //var passwordEncoder = new PasswordHasher<Account>().HashPassword(account, request.Password);
            //account.PasswordHash = passwordEncoder;
            //await _context.Account.AddAsync(account);
            //await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");

        }



        public async Task<ApiResponse> DeleteAccountAsync(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) throw new NotFoundException("Nhân viên này không tồn tại");
            account.IsDeleted = true;
            account.Phone = account.Phone + account.Id + "00";
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Xóa nhân viên thành công");
        }
        public async Task<ListAccountDto> GetAccountAsync
            (int lastId, int size, string? key)
        {
            var query = _context.Account.OrderBy(x => x.Id)
         .Where(ac => ac.Id > lastId && ac.IsDeleted == false);


            if (!string.IsNullOrEmpty(key))
            {
                string seachname = UtilService.NormalizeText(key);
                query = query.Where(ac =>
                     ac.SearchName.Contains(seachname) ||
                    EF.Functions.ILike(ac.Phone, $"%{key}%"));
            }

            _logger.LogInformation(query.ToQueryString());

            var accounts = await query

                .Take(size)
                .Select(ac => new GetAccountDTO(
                    ac.Id,
                    ac.FullName,
                    ac.Phone,
                    ac.Role.RoleName,
                    ac.IsDeleted))
                .ToListAsync();


            bool hasMore = accounts.Count == size;
            return new ListAccountDto(accounts, hasMore);
        }

        public async Task<List<HistoryOrderUserDto>> HistoryOrderUserDto(int accountId, int lastId = 0, int size = 3)
        {

            var query = await _context.Order
                .OrderByDescending(o => o.Id)
                .Where(o => o.OrderedUser == accountId && (lastId == 0 || o.Id < lastId))
                .Take(size)
                .Select(o => new HistoryOrderUserDto // Dùng ngoặc nhọn { } ở đây
                {
                    Id = o.Id,
                    Status = o.Status.StatusName,
                    Total = o.Total,
                    CreatedAt = o.CreatedAt, // Chuyển từ DateTimeOffset sang DateTime nếu DTO yêu cầu
                    Items = o.OrdersItems.Select(oi => new HistoryItemDto // Dùng ngoặc nhọn { } ở đây
                    {
                        FoodName = oi.Food.FoodName,
                        Price = oi.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return query;
        }
        public async Task<ApiResponse> UpdateAccountAsync(int id, UpdateAccountDto request)
        {
            //var account = await _context.Account.FindAsync(id);
            //if (account == null) throw new NotFoundException("Nhân viên này không tồn tại");

            //account.FullName = request.fullname;
            //account.SearchName = UtilService.NormalizeText(request.fullname);
            //account.Phone = request.Phone;
            //account.Role = request.Role;
            //account.UpdatedAt = DateTime.UtcNow;
            //_context.Account.Update(account);
            //await _context.SaveChangesAsync();
            return new ApiResponse(200, "Cập nhật thành công");
        }




    }
}
