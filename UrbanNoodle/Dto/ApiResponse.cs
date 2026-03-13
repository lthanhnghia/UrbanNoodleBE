namespace UrbanNoodle.Dto
{
    public class ApiResponse
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public ApiResponse()
        {
        }

        public ApiResponse(int Status, string Description)
        {
            this.Status = Status;
            this.Description = Description;
        }
    }
}
