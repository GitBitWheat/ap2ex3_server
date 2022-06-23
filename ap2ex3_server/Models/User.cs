using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ap2ex3_server.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A username is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "A nickname is required")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$", ErrorMessage = "Passwords contain at least 1 lower case letter, 1 uppercase letter and 1 digit")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}