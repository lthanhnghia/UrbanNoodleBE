using UrbanNoodle.Dto;
using UrbanNoodle.Dto.Order;

namespace UrbanNoodle.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse> CreateOrderAsync(CreateOrderDto request);

        Task<IEnumerable<GetOrderDto>> GetOrderAsync(int lastId, string? statusName, int size);

        Task<ApiResponse> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto request);
    }
}
