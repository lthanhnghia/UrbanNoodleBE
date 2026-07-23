using Microsoft.Extensions.Logging.Abstractions;
using UrbanNoodle.Dto.Order;
using UrbanNoodle.Services;
using UrbanNoodleTest.TestHelpers;

namespace UrbanNoodleTest.Service
{
    public class OrderServiceTests
    {

        [Fact]
        public async Task AddOrder_ShouldReturnTrue_WhenOrderIsValid()
        {
            var context = DbContextFactory.CreateInMemoryContext();
            var logger = NullLogger<OrderService>.Instance;

            var orderService = new OrderService(context, logger);

            var orderItem = new OrderItemDto { FoodId = 1, Quantity = 2 };
            var order = new CreateOrderDto
            {
                AddressId = 1,
                AccountId = 1,
                Item = new List<OrderItemDto> { orderItem }
            };
            var result = await orderService.CreateOrderAsync(order);
            Console.WriteLine($"Status: {result.Status}, Message: {result.Description}");
            Assert.Equal(404, result.Status);
        }


    }
}
