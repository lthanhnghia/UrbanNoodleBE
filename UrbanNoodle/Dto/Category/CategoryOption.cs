namespace UrbanNoodle.Dto.Category
{
    public class CategoryOption
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CategoryOption(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
