using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrbanNoodle.Entities
{
    [Table("chat_message")]
    public class ChatMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }


        [Column("ai_text")]
        public string AiText { get; set; }

        [Column("client_text")]
        public string ClientText { get; set; }

        [Column("roles")]
        public string Roles { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public ChatMessages()
        {
        }
    }
}
