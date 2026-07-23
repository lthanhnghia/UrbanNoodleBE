using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface IAlService
    {
        Task<ApiResponse> ChatAsync(string text, int? accountId);
        Task<ApiResponse> Embedding();
        Task<ApiResponse> SearchTopK(string text);
    }
}
