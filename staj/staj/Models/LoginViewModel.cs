using System.ComponentModel.DataAnnotations;

namespace staj.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username zorunludur")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; }
    }
}
