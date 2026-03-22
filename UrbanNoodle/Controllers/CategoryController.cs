using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Dto;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Dto.Category;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryService _category;
        public CategoryController(ICategoryService category) { 
             _category = category;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CategoryDto request)
        {

            var result = await _category.CreateCategoryAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }
        [HttpGet]
        public async Task<IEnumerable<GetCategoryDto>> GetCategory(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,
        [FromQuery] bool isDelete = false,
        [FromQuery] string? key = null)
        {

            return await _category.GetCategoryAsync(lastId, size, isDelete, key);
        }

        [HttpGet("options")]
        public async Task<IEnumerable<GetCategoryDto>> GetOptionCategory()
        {

            return await _category.GetOptionCategoryAsync();
        }

        [HttpPut("{ID}")]
        public async Task<ActionResult<ApiResponse>> UpdateCategory(int ID, [FromBody] CategoryDto request)
        {
            var result = await _category.UpdateCategoryAsync(ID, request);
            return new ApiResponse(result.Status, result.Description);

        }

        [HttpDelete("{ID}")]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(int ID)
        {
            var result = await _category.DeleteCategoryAsync(ID);
            return new ApiResponse(result.Status, result.Description);

        }
    }
}
