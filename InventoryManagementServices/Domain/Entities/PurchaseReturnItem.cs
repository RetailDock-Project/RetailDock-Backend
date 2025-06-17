using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PurchaseReturnItem
    {
        public Guid Id { get; set; }
        //fr key
        public Guid PurchaseReturnId { get; set; }
        public Guid OriginalPurchaseItemId { get; set; }
        public Guid ProductId { get; set; }
        public int ReturnedQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }


        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UGST { get; set; }
        public decimal? SGST { get; set; }
        public decimal Discount { get; set; } = 0;

        public PurchaseReturn PurchaseReturn { get; set; }
        public Product Product { get; set; }
        public PurchaseItem PurchaseItem { get; set; }
    }
}
