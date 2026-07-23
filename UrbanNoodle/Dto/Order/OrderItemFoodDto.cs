namespace UrbanNoodle.Dto.Order
{
    public class OrderItemFoodDto
    {
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public OrderItemFoodDto()
        {
        }
    }
}
