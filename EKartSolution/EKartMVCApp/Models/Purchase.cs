using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EKartMVCApp.Models
{
    public class Purchase
    {

        [Required(ErrorMessage = "Product Name is mandatory.")]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Purchase Id")]
        public long PurchaseId { get; set; }
        [DisplayName("Email Id")]
        public string EmailId { get; set; }
        [DisplayName("Product Id")]
        public string ProductId { get; set; }
        [DisplayName("Quantity purchased")]
        [Required(ErrorMessage = "Quantity purchased is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity purchased must be greater than 0")]
        public short QuantityPurchased { get; set; }
        [DisplayName("Date of purchase")]
        public System.DateTime DateOfPurchase { get; set; }
    }
}