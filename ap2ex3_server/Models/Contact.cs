using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ap2ex3_server.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Server { get; set; }

        public string? Last { get; set; } = null;

        public string? Lastdate { get; set; } = null;

        [Required]
        public List<Message> Messages { get; set; } = new List<Message>();

        [Required]
        public User? User { get; set; }
    }
}