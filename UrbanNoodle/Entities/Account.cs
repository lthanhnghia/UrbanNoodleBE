using System;
using System.Collections.Generic;
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

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("full_name")]
    public string FullName { get; set; } = null!;
    [Column("search_name")]
    public string SearchName { get; set; } = null!;

    [Column("phone")]
    public string Phone { get; set; } = null!;

    [Column("role")]
    public string Role { get; set; } = null!;

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("must_change_password")]
    public bool MustChangePassword { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    

    public Account(string fullName, string searchName ,string phone, string role, DateTime createdAt)
    {

        FullName = fullName;
        SearchName = searchName;
        Phone = phone;
        Role = role;
        CreatedAt = createdAt;
    }

    public Account()
    {
    }
}