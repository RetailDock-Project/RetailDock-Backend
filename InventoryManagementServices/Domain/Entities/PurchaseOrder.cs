using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Domain.Entities
{
    public class PurchaseOrder
    {

        public Guid OrganizationId { get; set; }
        public Guid PurchaseOrderId { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal GrossTotalAmount {  get; set; }

        public string OrderStatus {  get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Supplier Supplier { get; set; }

        public List<Purchase> Purchases { get; set; }
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}
