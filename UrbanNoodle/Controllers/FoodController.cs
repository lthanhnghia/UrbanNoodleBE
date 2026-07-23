using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Services.Interface;

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

        [Authorize(Roles = "admin")]
        [HttpPost]
        [EnableRateLimiting("AdminPolicy")]
        public async Task<ActionResult<ApiResponse>> CreateFood([FromForm] CreateFoodDto request)
        {

            var result = await _foodService.CreateFoodAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{ID}")]
        [EnableRateLimiting("AdminPolicy")]
        public async Task<ActionResult<ApiResponse>> UpdateFood(int ID, [FromForm] UpdateFoodDto request)
        {
            var result = await _foodService.UpdateFoodAsync(ID, request);
            return new ApiResponse(result.Status, result.Description);

        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{ID}")]
        [EnableRateLimiting("AdminPolicy")]
        public async Task<ActionResult<ApiResponse>> DeleteFood(int ID)
        {
            var result = await _foodService.DeleteFoodAsync(ID);
            return new ApiResponse(result.Status, result.Description);

        }

        [HttpGet]
        public async Task<ListFood> GetFood(
        [FromQuery] int lastId = 0,
        [FromQuery] int size = 3,
        [FromQuery] string? key = null)
        {

            return await _foodService.GetFoodAsync(lastId, size, key);
        }
    }
}
