using System;
using System.Collections.Generic;

namespace UrbanNoodle.Entities;

public partial class Food
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;
}
