using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto.Food;
using UrbanNoodle.Dto;
using UrbanNoodle.Services;
using UrbanNoodle.Services.Interface;
using UrbanNoodle.Dto.Order;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateFood([FromBody] CreateOrderDto request)
        {

            var result = await _orderService.CreateOrderAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }
    }
}
