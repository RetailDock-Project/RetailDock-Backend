using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class AddPurchaseOrderDto
    {
        public Guid SupplierId { get; set; }
     
        public Guid CreatedBy { get; set; }
        public List<AddPurchaseOrderItemDto> Items { get; set; }
    }

    public class AddPurchaseOrderItemDto
    {
        
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal RatePerPiece { get; set; }
    }
    public class PurchaseOrderDto
    {
        public Guid PurchaseOrderId { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseOrderNumber { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal GrossTotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public Guid CreatedBy { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; }
    }
    public class PurchaseOrderItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal RatePerPiece { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class UpdateOrderStatusDto
    {
        [RegularExpression("Pending|Cancelled|Completed", ErrorMessage = "Invalid Status")]
        public string OrderStatus { get; set; }
    }
}
