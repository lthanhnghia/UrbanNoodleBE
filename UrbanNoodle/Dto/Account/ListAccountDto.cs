namespace UrbanNoodle.Dto.Account
{
    public class ListAccountDto
    {
        public ICollection<GetAccountDTO> Data { get; set; }
        public bool HasMore { get; set; }

        public ListAccountDto(ICollection<GetAccountDTO> data, bool hasMore)
        {
            Data = data;
            HasMore = hasMore;
        }
    }
}
