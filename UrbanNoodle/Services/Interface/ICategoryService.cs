using UrbanNoodle.Dto.Category;
using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface ICategoryService
    {
        Task<ApiResponse> CreateCategoryAsync(CategoryDto request);
        Task<ListCategoryDto> GetCategoryAsync(int lastId, int size, string? key);
        Task<IEnumerable<CategoryOption>> GetOptionCategoryAsync();
        Task<ApiResponse> UpdateCategoryAsync(int id, CategoryDto request);
        Task<ApiResponse> DeleteCategoryAsync(int id);
    }
}
