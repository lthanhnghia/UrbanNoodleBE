namespace UrbanNoodle.Dto.Order
{
    public class CreateOrderDto
    {
        public int AccountId { get; set; }
        public int DiningTableId { get; set; }
        public List<OrderItemDto> Item { get; set; }
    }
}
