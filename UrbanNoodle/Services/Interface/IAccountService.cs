using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Account;

namespace UrbanNoodle.Service.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse> CreateAccountAsync(AccountDTO request);
        Task<ListAccountDto> GetAccountAsync
            (int lastId = 0, int size = 5, string? key = null);
        Task<ApiResponse> UpdateAccountAsync(int id, UpdateAccountDto request);
        Task<ApiResponse> DeleteAccountAsync(int id);
        Task<List<HistoryOrderUserDto>> HistoryOrderUserDto(int? id, int accountId, int lastId = 0, int size = 3);

    }
}
