using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Category;

namespace UrbanNoodle.Services.Interface
{
    public interface IFoodService
    {
        Task<ApiResponse> CreateFoodAsync(CreateFoodDto request);
        Task<IEnumerable<GetFoodDto>> GetFoodAsync(int lastId, int size, bool isDelete, string? key);

        Task<ApiResponse> UpdateFoodAsync(int id, UpdateFoodDto request);
        Task<ApiResponse> DeleteFoodAsync(int id);
    }
}
