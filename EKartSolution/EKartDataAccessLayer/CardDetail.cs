//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EKartDataAccessLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class CardDetail
    {
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string EmailId { get; set; }
        public string CardType { get; set; }
        public decimal CVVNumber { get; set; }
        public System.DateTime ExpiryDate { get; set; }
        public Nullable<decimal> Balance { get; set; }
    
        public virtual User User { get; set; }
    }
}
