using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Dto.Category
{
    public class CategoryDto
    {
        [Required(ErrorMessage = "Tên phân loại không được để trống.")]
        [StringLength(40, MinimumLength = 1,
        ErrorMessage = "Tên phân loại phải từ 1 đến 40 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(150, MinimumLength = 10,
        ErrorMessage = "Mô tả phải từ 10 đến 150 ký tự.")]
        public string Description { get; set; }
    }
}
