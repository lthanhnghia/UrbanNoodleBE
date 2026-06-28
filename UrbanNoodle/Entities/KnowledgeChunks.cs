using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Pgvector;


namespace UrbanNoodle.Entities;

    [Table("knowledge_chunks")]
    public class KnowledgeChunks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }


        [Column("content")]
        public string Content { get; set; }

        [Column("embedding")]
        public Vector Embedding { get; set; }
    }

