using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Entities;


[Table("food")]
public class Food
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("search_name")]
    public string SearchName { get; set; } 

    [Column("price")]
    public decimal Price { get; set; }

    [Column("image_url")]
    public string ImageUrl { get; set; } = null!;

    [Column("status")]
    public bool Status { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;
}
