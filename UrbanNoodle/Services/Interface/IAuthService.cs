using System.Threading.Tasks;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Auth;

namespace UrbanNoodle.Service.Interface
{
    public interface IAuthService
    {
        Task<ResponseLoginDTO> LoginAsync(LoginDto request);
        Task<ApiResponse> RegisterAsync(RegisterDto request);
    }
}
