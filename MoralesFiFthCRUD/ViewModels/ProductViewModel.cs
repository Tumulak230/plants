using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoralesFiFthCRUD.ViewModels
{
    public class ProductViewModel
    {
        public int UserId { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public byte[] ProductImg { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string sellerName { get; set; }
        public string ProductOwner { get; set; }
    }
}