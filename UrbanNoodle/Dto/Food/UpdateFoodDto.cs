namespace UrbanNoodle.Dto.Food
{
    public class UpdateFoodDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
