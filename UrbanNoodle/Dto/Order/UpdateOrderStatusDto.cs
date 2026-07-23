using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Dto.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [RegularExpression("^(ordered|confirmed|success|cancelled)$",
        ErrorMessage = "Trạng thái không hợp lệ")]
        public string StatusName { get; set; } = string.Empty;
        public int ConfirmedBy { get; set; }

        public UpdateOrderStatusDto()
        {
        }
    }
}
