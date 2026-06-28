using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Auth;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Utils;

namespace UrbanNoodle.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ResponseLoginDTO> LoginAsync(LoginDto request)
        {
            var account = await _context.Account.Include(x => x.Role)
          .FirstOrDefaultAsync(ac => ac.Phone == request.key || ac.UserName == request.key);

            // Để bảo mật, không báo cụ thể là sai ID hay sai Pass, cứ báo chung là thất bại
            if (account == null)
            {
                throw new UnauthorizedException("Đăng nhập thất bại");
            }

            // 2. Kiểm tra mật khẩu (Sử dụng trực tiếp phiên bản xác thực của PasswordHasher)
            var passwordHasher = new PasswordHasher<Account>();
            var verificationResult = passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Đăng nhập thất bại");
            }
            return new ResponseLoginDTO(200, "Đăng nhập thành công", CreateToken(account));
        }

        public async Task<ApiResponse> RegisterAsync(RegisterDto request)
        {
            var existingAccount = await _context.Account
                           .FirstOrDefaultAsync(u => u.Phone == request.Phone
                           || u.UserName == request.UserName
                           || u.Email == request.Email);


            if (existingAccount != null)
            {
                if (existingAccount.Phone == request.Phone)
                {
                    throw new BadRequestException("Số điện thoại này đã được sử dụng");
                }

                if (existingAccount.UserName == request.UserName)
                {
                    throw new BadRequestException("Tên đăng nhập này đã được sử dụng");
                }

                if (existingAccount.Email == request.Email)
                {
                    throw new BadRequestException("Email này đã được sử dụng");
                }
            }

            string SearchName = UtilService.NormalizeText(request.FullName);
            var account = new Account(
              request.UserName,
              request.FullName,
              request.Email,
              UtilService.NormalizeText(request.FullName),
              request.Phone,
              2,
              DateTime.UtcNow
          );
            var passwordEncoder = new PasswordHasher<Account>().HashPassword(account, request.Password);
            account.PasswordHash = passwordEncoder;
            await _context.Account.AddAsync(account);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Thêm mới thành công");
        }

        private String CreateToken(Account account)
        {
            var list = new List<Claim>
            {
                new Claim("id",account.Id.ToString()),
                new Claim("phone",account.Phone),
                new Claim("role",account.Role.RoleName)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT_SECRET")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWT_ISSUER"),
                audience: _configuration.GetValue<string>("JWT_AUDIENCE"),
                claims: list,
                expires: DateTime.UtcNow.AddMinutes(45),
                signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
