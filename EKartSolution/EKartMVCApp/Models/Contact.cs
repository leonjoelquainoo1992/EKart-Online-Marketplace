using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class Contact
    {
        [Required(ErrorMessage = "First is mandatory.")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is mandatory.")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "EmailId is mandatory.")]
        [DisplayName("Email Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Phone Number is mandatory.")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = " Message is mandatory.")]
        [DisplayName("Message")]
        public string Message { get; set; }
    }
}