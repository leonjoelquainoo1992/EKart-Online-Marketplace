using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class Login
    {
        [Required(ErrorMessage = "EmailId is mandatory.")]
        [DisplayName("Email Id")]
        public string EmailId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is mandatory.")]
        [DisplayName("User Password")]
        public string UserPassword { get; set; }

        [DisplayName("Remember Me")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        public string RememberMe { get; set; }
    }
}