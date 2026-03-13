using UrbanNoodle.Dto;

namespace UrbanNoodle.Service.Interface
{
    public interface IAuthService
    {
        Task<ResponseLoginDTO> LoginAsync(LoginDto request);
    }
}
