namespace UrbanNoodle.Dto
{
    public class ResponseAccountDTO
    {
        public int id { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public string role { get; set; }
        public bool isdelete { get; set; }

        public ResponseAccountDTO(int id, string fullname, string phone, string role, bool isdelete)
        {
            this.id = id;
            this.fullname = fullname;
            this.phone = phone;
            this.role = role;
            this.isdelete = isdelete;
        }
    }
}
