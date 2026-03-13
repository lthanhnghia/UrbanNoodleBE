using System;
using System.Collections.Generic;

namespace UrbanNoodle.Entities;

public partial class OrdersItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int OrdersId { get; set; }

    public int FoodId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account Orders { get; set; } = null!;

    public virtual Order OrdersNavigation { get; set; } = null!;
}
