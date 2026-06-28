using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Dto.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải từ 4 đến 15 ký tự.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự để đảm bảo bảo mật.")]
        public string Password { get; set; } = null!; 

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "Số điện thoại phải từ 9 đến 15 ký tự.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [StringLength(40, ErrorMessage = "Email không được vượt quá 40 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = null!;
    }
}
