using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UrbanNoodle.Entities;

[Table("orders")]
public  class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("diningtables_id")]
    public int DiningtablesId { get; set; }

    [Column("accounts_id")]
    public int AccountsId { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("paid_at")]
    public DateTime? PathAt { get; set; }

    public virtual Account Accounts { get; set; } = null!;

    public virtual DiningTable Diningtables { get; set; } = null!;

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
