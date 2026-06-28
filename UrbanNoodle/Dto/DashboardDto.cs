using UrbanNoodle.Dto.Food;

namespace UrbanNoodle.Dto
{
    public class DashboardDto
    {
        public decimal TotalAmount {  get; set; }
        public int NumberOrder { get; set; }

        public string FoodName { get; set; }
        public int TotalSold { get; set; }
        public int NumberAccount { get; set; }
        public ICollection<TopFoodDto> TopFood { get; set; }

        public DashboardDto(decimal totalAmount, int numberOrder, string foodName, int totalSold, int numberAccount, ICollection<TopFoodDto> topFood)
        {
            TotalAmount = totalAmount;
            NumberOrder = numberOrder;
            FoodName = foodName;
            TotalSold = totalSold;
            NumberAccount = numberAccount;
            TopFood = topFood;
        }
    }
}
