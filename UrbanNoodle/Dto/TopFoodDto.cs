namespace UrbanNoodle.Dto
{
    public class TopFoodDto
    {
        public string FoodName { get; set; } 
        public int TotalOrders { get; set; }
        public decimal Revenue { get; set; }

        public TopFoodDto(string foodName, int totalOrders, decimal revenue)
        {
            FoodName = foodName;
            TotalOrders = totalOrders;
            Revenue = revenue;
        }
    }
}
