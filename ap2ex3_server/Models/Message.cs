using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ap2ex3_server.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool Sent { get; set; }

        [Required]
        public Contact? Contact { get; set; }
    }
}