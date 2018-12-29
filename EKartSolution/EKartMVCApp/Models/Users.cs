using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class Users
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

        [DataType(DataType.Password)]
        [StringLength(maximumLength: 15,MinimumLength = 8, ErrorMessage = "Password length must be between 8 and 15")]
        [Required(ErrorMessage = "Password is mandatory.")]
        [DisplayName("User Password")]
        public string UserPassword { get; set;}

        [DataType(DataType.Password)]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Password length must be between 8 and 15")]
        [Required(ErrorMessage = "Confirm Password is mandatory.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Old Password is mandatory.")]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }


        [ScaffoldColumn(false)]
        public Nullable<byte> RoleId { get; set; }

        [Required(ErrorMessage = "Phone Number is mandatory.")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "DOB is mandatory.")]
        [DataType(DataType.DateTime)]
        [DisplayName("Date of Birth")]
        public System.DateTime DateOfBirth {get; set; }

        [Required(ErrorMessage = "AddressLine is mandatory.")]
        [DisplayName("Address Line1")]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "City is mandatory.")]
        [DisplayName("City")]
        public string City { get; set;}

        [Required(ErrorMessage = "State is mandatory.")]
        [DisplayName("State")]
        public string State { get; set; }

        [Required(ErrorMessage = "ZipCode is mandatory.")]
        [DisplayName("ZipCode")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is mandatory.")]
        [DisplayName("Country")]
        public string Country { get; set; }
    }
}