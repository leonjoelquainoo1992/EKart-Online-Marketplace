using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EKartMVCApp.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Product Id is mandatory.")]
        [DisplayName("Product Id")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "Price is mandatory.")]
        [DisplayName("Price")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Category Id is mandatory.")]
        [DisplayName("Category Id")]
        public byte? CategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is mandatory.")]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity Available should be greater than 0.")]
        [DisplayName("Quantity Available")]
        public int QuantityAvailable { get; set; }
    }
}