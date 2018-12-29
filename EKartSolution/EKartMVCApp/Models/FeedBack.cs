using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class FeedBack
    {
        [Required(ErrorMessage = "Field is mandatory.")]
        [DisplayName("Overall Shopping Experience")]
        public string Ratings { get; set; }
    }
}