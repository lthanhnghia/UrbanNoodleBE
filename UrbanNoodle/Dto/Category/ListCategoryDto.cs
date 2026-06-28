using UrbanNoodle.Dto.Account;

namespace UrbanNoodle.Dto.Category
{
    public class ListCategoryDto
    {
        public ICollection<GetCategoryDto> Data { get; set; }
        public bool HasMore { get; set; }

        public ListCategoryDto(ICollection<GetCategoryDto> data, bool hasMore)
        {
            Data = data;
            HasMore = hasMore;
        }
    }
}
