using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Entities;
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
            if (await _context.Account.AnyAsync(u => u.Phone.Equals(request.Phone)))
            {
                throw new BadRequestException("Số điện thoại này đã được sử dụng");
            }
            string SearchName = UtilService.NormalizeText(request.FullName);
            var account = new Account(request.FullName, SearchName, request.Phone, request.Role, DateTime.UtcNow);
            var passwordEncoder = new PasswordHasher<Account>().HashPassword(account, request.Password);
            account.PasswordHash = passwordEncoder;
            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");

        }

        public async Task<ApiResponse> DeleteAccountAsync(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) throw new NotFoundException("Nhân viên này không tồn tại");
            account.IsDeleted = true;
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Xóa nhân viên thành công");
        }

        public async Task<IEnumerable<GetAccountDTO>> GetAccountAsync
            (int lastId, int size, bool isDelete, string? key)
        {
            var query = _context.Account
         .Where(ac => ac.Id > lastId && ac.IsDeleted == isDelete);

            if (!string.IsNullOrEmpty(key))
            {
                string seachname = UtilService.NormalizeText(key);
                query = query.Where(ac =>
                     ac.SearchName.Contains(seachname) ||
                    EF.Functions.ILike(ac.Phone, $"%{key}%"));
            }

            _logger.LogInformation(query.ToQueryString());

            var accounts = await query
                .OrderBy(ac => ac.Id)
                .Take(size)
                .Select(ac => new GetAccountDTO(
                    ac.Id,
                    ac.FullName,
                    ac.Phone,
                    ac.Role,
                    ac.IsDeleted))
                .ToListAsync();

            return accounts;
        }

        public async Task<ApiResponse> UpdateAccountAsync(int id, UpdateAccountDto request)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) throw new NotFoundException("Nhân viên này không tồn tại");

            account.FullName = request.Fullname;
            account.SearchName = UtilService.NormalizeText(request.Fullname);
            account.Phone = request.Phone;
            account.Role = request.Role;
            account.UpdatedAt = DateTime.UtcNow;
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Cập nhật thành công");
        }
    }
}
