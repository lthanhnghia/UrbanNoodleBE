using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Service.Interface;

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
            if (await _context.Account.AnyAsync(u => u.Phone.Equals(request.phone)))
            {
                throw new BadRequestException("Số điện thoại này đã được sử dụng");
            }
            var account = new Account(request.fullName, request.phone, request.role, DateTime.UtcNow);
            var passwordEncoder = new PasswordHasher<Account>().HashPassword(account, request.password);
            account.PasswordHash = passwordEncoder;
            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");

        }



        public async Task<IEnumerable<ResponseAccountDTO>> GetAccountAsync
            (int lastId, int size, bool isDelete, string? key)
        {
            var query = _context.Account
         .Where(ac => ac.Id > lastId && ac.IsDeleted == isDelete);

            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(ac =>
                    EF.Functions.ILike(ac.FullName, $"%{key}%") ||
                    EF.Functions.ILike(ac.Phone, $"%{key}%"));
            }

            _logger.LogInformation(query.ToQueryString());

            var accounts = await query
                .OrderBy(ac => ac.Id)
                .Take(size)
                .Select(ac => new ResponseAccountDTO(
                    ac.Id,
                    ac.FullName,
                    ac.Phone,
                    ac.Role,
                    ac.IsDeleted))
                .ToListAsync();

            return accounts;
        }
    }
}
