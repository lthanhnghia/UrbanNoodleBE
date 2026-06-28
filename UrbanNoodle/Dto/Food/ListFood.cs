
namespace UrbanNoodle.Dto.Food
{
    public class ListFood
    {
        public ICollection<GetFoodDto> Data { get; set; }
        public bool HasMore { get; set; }

        public ListFood(ICollection<GetFoodDto> data, bool hasMore)
        {
            Data = data;
            HasMore = hasMore;
        }
    }
}
