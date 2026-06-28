namespace UrbanNoodle.Dto
{
    public class ChatMessageDto
    {
        public string role { get; set; }
        public List<PartDto> parts { get; set; } = new();
    }
}
