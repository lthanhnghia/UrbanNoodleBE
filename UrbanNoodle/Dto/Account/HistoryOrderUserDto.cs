namespace UrbanNoodle.Dto.Account
{
    public class HistoryOrderUserDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HistoryItemDto> Items { get; set; } = new();



        public HistoryOrderUserDto()
        {
        }
    }
}
