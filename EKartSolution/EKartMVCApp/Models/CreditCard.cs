using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class CreditCard
    {
        [Required(ErrorMessage = "Name is mandatory.")]
        [DisplayName("Name On Card")]
        public string NameOnCard { get; set; }

        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "EmailId is mandatory.")]
        [DisplayName("Email Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Card Type is mandatory.")]
        [DisplayName("CardType")]
        public string CardType { get; set; }

        [StringLength(maximumLength: 16, MinimumLength = 16, ErrorMessage = "Invalid Credit Card Number")]
        [Required(ErrorMessage = "Card Number is mandatory.")]
        [DisplayName("Card Number")]
        public string CreditCardNumber { get; set; }

        [Required(ErrorMessage = "CVV is mandatory.")]
        [DisplayName("CVV")]
        public decimal CVV { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Expiry Date is mandatory.")]
        [DisplayName("ExpiryDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }
    }
}