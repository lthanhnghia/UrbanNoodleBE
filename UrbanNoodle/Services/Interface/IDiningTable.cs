using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.DiningTable;

namespace UrbanNoodle.Services.Interface
{
    public interface IDiningTable
    {
        Task<ApiResponse> CreateDiningTableAsync(CreateDiningTableDto request);
        Task<IEnumerable<GetDiningTableDto>> GetDiningTableAsync(int lastId, int size, bool isDelete, bool? status ,string? key);
        Task<ApiResponse> UpdateDiningTableAsync(int id, UpdateDiningTableDto request);
        Task<ApiResponse> DeleteDiningTableAsync(int id);
    }
}
