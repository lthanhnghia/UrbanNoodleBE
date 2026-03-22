using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Account;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Service.Interface;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Dto.Category;
using UrbanNoodle.Entities;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateFood([FromForm] CreateFoodDto request)
        {

            var result = await _foodService.CreateFoodAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }
        [HttpPut("{ID}")]
        public async Task<ActionResult<ApiResponse>> UpdateFood(int ID,[FromForm] UpdateFoodDto request)
        {
            var result = await _foodService.UpdateFoodAsync(ID,request);
            return new ApiResponse(result.Status, result.Description);

        }
        [HttpDelete("{ID}")]
        public async Task<ActionResult<ApiResponse>> DeleteFood(int ID)
        {
            var result = await _foodService.DeleteFoodAsync(ID);
            return new ApiResponse(result.Status, result.Description);

        }
        [HttpGet]
        public async Task<IEnumerable<GetFoodDto>> GetFood(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,
        [FromQuery] bool isDelete = false,
        [FromQuery] string? key = null)
        {

            return await _foodService.GetFoodAsync(lastId, size, isDelete, key);
        }
    }
}
