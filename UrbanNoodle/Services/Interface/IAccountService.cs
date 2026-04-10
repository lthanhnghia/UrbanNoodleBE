using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Dto.Category;

namespace UrbanNoodle.Service.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse> CreateAccountAsync(AccountDTO request);
        Task<ListAccountDto> GetAccountAsync
            (int lastId = 0, int size = 5, bool isDelete = false, string? key = null);
        Task<ApiResponse> UpdateAccountAsync(int id,UpdateAccountDto request);
        Task<ApiResponse> DeleteAccountAsync(int id);
    }
}
