using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PurchaseReturn
    {
        public Guid Id { get; set; }
        public Guid OriginalPurchaseId { get; set; }

        public DateOnly ReturnDate { get; set; }
        public Guid PurchaseReturnInvoiceId { get; set; }
        public Guid SupplierId { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid OrganizationId { get; set; }


        public PurchaseReturnInvoice PurchaseReturnInvoice { get; set; }

        public Purchase Purchase { get; set; }

        public List<PurchaseReturnItem> Items { get; set; }
        public Supplier Supplier { get; set; }

    }
}
