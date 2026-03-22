using Npgsql;

namespace UrbanNoodle.Dto.Order
{
    public class OrderItemDto
    {
        public int FoodId { get; set; }
        public int Quantity {  get; set; }

    }
}
