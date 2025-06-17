using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PurchaseOrderItem
    {
        public Guid PurchaseOrderItemId { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity {  get; set; }
        public decimal RatePerPiece {  get; set; }
        public decimal TotalAmount {  get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public Product Product { get; set; }
        
    
}
}
