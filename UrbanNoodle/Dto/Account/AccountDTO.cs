using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Dto.Account
{
    public class AccountDTO
    {
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(50, MinimumLength = 3,
         ErrorMessage = "Tên phải từ 1 đến 50 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$",
         ErrorMessage = "Tên chỉ được chứa chữ cái.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone]
        [RegularExpression(@"^0\d{9}$",
           ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Phân quyền không được để trống.")]
        [RegularExpression("^(admin|staff|user)$",
        ErrorMessage = "Phân quyền không hợp lệ")]
        public string Role { get; set; }

    }
}
