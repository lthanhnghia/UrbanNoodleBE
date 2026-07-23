using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanNoodle.Entities;

[Table("accounts")]
public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }


    [Column("user_name")]
    public string UserName { get; set; } = null!;


    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("full_name")]
    public string FullName { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("search_name")]
    public string SearchName { get; set; } = null!;

    [Column("phone")]
    public string Phone { get; set; } = null!;

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; }

    [InverseProperty("OrderedByUser")]
    public virtual ICollection<Order> OrderedUser { get; set; } = new List<Order>();

    [InverseProperty("ConfirmedByAdmin")]
    public virtual ICollection<Order> ConfirmedAdmin { get; set; } = new List<Order>();


    [InverseProperty("Account")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();


    [InverseProperty("Account")]
    public virtual ICollection<ChatMessages> ChatMessage { get; set; } = new List<ChatMessages>();

    public Account(string userName, string fullName, string email, string searchName, string phone, int roleId, DateTime createdAt)
    {
        UserName = userName;
        FullName = fullName;
        Email = email;
        SearchName = searchName;
        Phone = phone;
        RoleId = roleId;
        CreatedAt = createdAt;
    }

    public Account()
    {
    }
}