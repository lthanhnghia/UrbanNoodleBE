using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanNoodle.Entities;

[Table("orders")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }


    [Column("ordered_user")]
    public int OrderedUser { get; set; }

    [ForeignKey("OrderedUser")]
    public virtual Account OrderedByUser { get; set; } = null!;



    [Column("confirmed_admin")]
    public int? ConfirmedAdmin { get; set; }

    [ForeignKey("ConfirmedAdmin")]
    public virtual Account? ConfirmedByAdmin { get; set; }


    [Column("total")]
    public decimal Total { get; set; }


    [Column("status_id")]
    public int StatusId { get; set; }

    [ForeignKey("StatusId")]
    public virtual Status Status { get; set; } = null!;


    [Column("address_id")]
    public int? AddressId { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }






    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();
}
