using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Dto.Food
{
    public class UpdateFoodDto
    {
        [Required(ErrorMessage = "Tên món ăn không được để trống.")]
        [StringLength(40, MinimumLength = 1,
        ErrorMessage = "Tên món ăn phải từ 1 đến 40 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá món ăn không được để trống")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
        ErrorMessage = "Giá món ăn phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Danh mục phải hợp lệ.")]
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
