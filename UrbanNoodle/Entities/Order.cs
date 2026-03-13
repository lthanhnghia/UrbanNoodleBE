using System;
using System.Collections.Generic;

namespace UrbanNoodle.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int DiningtablesId { get; set; }

    public int AccountsId { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account Accounts { get; set; } = null!;

    public virtual DiningTable Diningtables { get; set; } = null!;

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
