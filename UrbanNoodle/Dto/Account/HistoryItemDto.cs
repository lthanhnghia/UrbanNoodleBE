namespace UrbanNoodle.Dto.Account
{
    public class HistoryItemDto
    {
        public string FoodName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }



        public HistoryItemDto()
        {
        }
    }
}
