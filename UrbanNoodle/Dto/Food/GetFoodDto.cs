namespace UrbanNoodle.Dto.Food
{
    public class GetFoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string image { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }

        public GetFoodDto(int id, string name, decimal price, string image, string categoryName)
        {
            Id = id;
            Name = name;
            Price = price;
            this.image = image;

            CategoryName = categoryName;
        }

        public GetFoodDto()
        {
        }
    }
}
