namespace UrbanNoodle.Dto
{
    public class ResponseLoginDTO
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }

        public ResponseLoginDTO(int status, string description, string token)
        {
            Status = status;
            Description = description;
            Token = token;
        }
    }
}
