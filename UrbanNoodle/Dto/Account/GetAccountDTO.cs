using UrbanNoodle.Dto.Address;

namespace UrbanNoodle.Dto.Account
{
    public class GetAccountDTO
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public bool Isdelete { get; set; }

        public List<AddressByAccount> Addresses { get; set; }
        public GetAccountDTO(int id, string fullname, string phone, string role, bool isdelete)
        {
            this.Id = id;
            this.Fullname = fullname;
            this.Phone = phone;
            this.Role = role;
            this.Isdelete = isdelete;
        }

        public GetAccountDTO()
        {
        }
    }
}
