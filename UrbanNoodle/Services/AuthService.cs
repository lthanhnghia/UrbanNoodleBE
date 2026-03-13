using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Entities;
using UrbanNoodle.Exceptions;
using UrbanNoodle.Service.Interface;

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
            var account = _context.Account.SingleOrDefault(ac => ac.Phone == request.phone);
            if (account == null)
            {
                throw new UnauthorizedException("Đăng nhập thất bại");
            }
            if(new PasswordHasher<Account>()
                .VerifyHashedPassword
                (account, account.PasswordHash, request.password) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Đăng nhập thất bại");
            }
            return new ResponseLoginDTO(200,"Đăng nhập thành công",CreateToken(account));
        }
        private String CreateToken(Account account)
        {
            var list = new List<Claim>
            {
                new Claim("id",account.Id.ToString()),
                new Claim("phone",account.Phone),
                new Claim("role",account.Role)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT_SECRET")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWT_ISSUER"),
                audience: _configuration.GetValue<string>("JWT_AUDIENCE"),
                claims: list,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: cred
                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
