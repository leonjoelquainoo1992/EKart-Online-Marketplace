using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EKartMVCApp.Models
{
    public class ShoppingCart
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<byte> CategoryId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}