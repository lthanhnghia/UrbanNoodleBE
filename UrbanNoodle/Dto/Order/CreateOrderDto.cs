namespace UrbanNoodle.Dto.Order
{
    public class CreateOrderDto
    {
        public int AccountId { get; set; }
        public int AddressId { get; set; }
        public List<OrderItemDto> Item { get; set; }
    }
}
