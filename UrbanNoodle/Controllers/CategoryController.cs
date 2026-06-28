using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Dto;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Dto.Category;
using UrbanNoodle.Service;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryService _category;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService category, ILogger<CategoryController> logger) { 
             _category = category;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CategoryDto request)
        {

            var result = await _category.CreateCategoryAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }
        [HttpGet]
        public async Task<ListCategoryDto> GetCategory(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,

        [FromQuery] string? key = null)
        {

            return await _category.GetCategoryAsync(lastId, size,  key);
        }

        [HttpGet("options")]
        public async Task<IEnumerable<CategoryOption>> GetOptionCategory()
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
