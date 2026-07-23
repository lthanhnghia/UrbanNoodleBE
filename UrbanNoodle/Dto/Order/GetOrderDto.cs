namespace UrbanNoodle.Dto.Order
{
    public class GetOrderDto
    {
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientAddress { get; set; }
        public int OrderId { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemFoodDto> Items { get; set; }

        public GetOrderDto()
        {
        }
    }
}
