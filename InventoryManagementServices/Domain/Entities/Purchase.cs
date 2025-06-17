using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public Guid PurchaseInvoiceId { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime Purchasedate { get; set; }
        public string? SupplierInvoiceNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid OrganizationId { get; set; }

        public Guid? PurchaseOrderId { get; set; }
        //public Guid? DocumentId { get; set; }

        //public Document? Document { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
        public List<PurchaseItem> PurchaseItems { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; }
        public List<PurchaseReturn> PurchaseReturns { get; set; }



        public Supplier Supplier { get; set; }


    }
}
