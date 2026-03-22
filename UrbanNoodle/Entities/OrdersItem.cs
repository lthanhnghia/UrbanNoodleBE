using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Entities;

[Table("orders_item")]
public class OrdersItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("orders_id")]
    public int OrdersId { get; set; }

    [Column("food_id")]
    public int FoodId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Order Orders{ get; set; } = null!;
}
