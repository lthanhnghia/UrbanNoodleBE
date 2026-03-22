using UrbanNoodle.Dto.Order;
using UrbanNoodle.Dto;

namespace UrbanNoodle.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse> CreateOrderAsync(CreateOrderDto request);
    }
}
