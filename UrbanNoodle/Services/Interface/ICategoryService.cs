using UrbanNoodle.Dto.Category;
using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface ICategoryService
    {
        Task<ApiResponse> CreateCategoryAsync(CategoryDto request);
        Task<IEnumerable<GetCategoryDto>> GetCategoryAsync(int lastId, int size, bool isDelete, string? key);
        Task<IEnumerable<GetCategoryDto>> GetOptionCategoryAsync();
        Task<ApiResponse> UpdateCategoryAsync(int id, CategoryDto request);
        Task<ApiResponse> DeleteCategoryAsync(int id);
    }
}
