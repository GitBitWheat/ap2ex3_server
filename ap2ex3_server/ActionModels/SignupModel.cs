using System.ComponentModel.DataAnnotations;
using ap2ex3_server.Models;

namespace ap2ex3_server.ActionModels
{
    public class SignupModel
    {
        [Required(ErrorMessage = "A username is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string? Id { get; set; }

        [Required(ErrorMessage = "A nickname is required")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "A password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).+$", ErrorMessage = "Passwords contain at least 1 lower case letter, 1 uppercase letter and 1 digit")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public User ConvertToUser()
        {
            return new User()
            {
                Username = Id,
                Name = Name,
                Password = Password
            };
        }
    }
}
