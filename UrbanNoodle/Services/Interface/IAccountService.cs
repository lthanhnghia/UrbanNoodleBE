using UrbanNoodle.Dto;

namespace UrbanNoodle.Service.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse> CreateAccountAsync(AccountDTO request);
        Task<IEnumerable<ResponseAccountDTO>> GetAccountAsync
            (int lastId = 0, int size = 5, bool isDelete = false, string? key = null);
    }
}
