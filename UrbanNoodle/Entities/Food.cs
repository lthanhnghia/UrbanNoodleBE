using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanNoodle.Entities;


[Table("food")]
public class Food
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("food_name")]
    public string FoodName { get; set; } = null!;

    [Column("search_name")]
    public string SearchName { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;

    public Food(string foodName, string searchName, decimal price, string imageUrl, bool isDeleted, int categoryId, DateTime createdAt)
    {
        FoodName = foodName;
        SearchName = searchName;
        Price = price;
        ImageUrl = imageUrl;
        IsDeleted = isDeleted;
        CategoryId = categoryId;
        CreatedAt = createdAt;
    }

    public Food()
    {
    }
}
