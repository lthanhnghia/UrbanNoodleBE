using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Category;

namespace UrbanNoodle.Services.Interface
{
    public interface IFoodService
    {
        Task<ApiResponse> CreateFoodAsync(CreateFoodDto request);
        Task<ListFood> GetFoodAsync(int lastId, int size,  string? key);

        Task<ApiResponse> UpdateFoodAsync(int id, UpdateFoodDto request);
        Task<ApiResponse> DeleteFoodAsync(int id);
        Task<IEnumerable<GetFoodDto>> GetAllFood();
    }
}
