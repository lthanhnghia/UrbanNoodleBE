using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Order;
using UrbanNoodle.Services.Interface;

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

        [Authorize(Roles = "client")]
        [HttpPost]
        [EnableRateLimiting("OrderPolicy")]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] CreateOrderDto request)
        {

            var result = await _orderService.CreateOrderAsync(request);
            return new ApiResponse(result.Status, result.Description);
        }

        [Authorize(Roles = "client")]
        [HttpPut("{ID}")]
        public async Task<ActionResult<ApiResponse>> UpdateStatusOrder(int ID, [FromForm] UpdateOrderStatusDto request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(ID, request);
            return new ApiResponse(result.Status, result.Description);

        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IEnumerable<GetOrderDto>> GetOrder(
            [FromQuery] int lastId = 0,
            [FromQuery] int size = 6,
            [FromQuery] string? statusName = null
         )
        {

            return await _orderService.GetOrderAsync(lastId, statusName, size);
        }
    }
}
