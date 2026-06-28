using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface IAlService
    {
        Task<ApiResponse> ChatAsync(string text, int? accountId);
    }
}
