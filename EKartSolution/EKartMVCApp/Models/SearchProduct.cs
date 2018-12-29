using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EKartMVCApp.Models
{
    public class SearchProduct
    {
        [Required(ErrorMessage = "Field is mandatory.")]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
    }
}