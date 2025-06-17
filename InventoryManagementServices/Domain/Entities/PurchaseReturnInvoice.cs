using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class PurchaseReturnInvoice
    {
        public Guid Id { get; set; }

        // Auto-generated like "PRI-2025-0001"
        public string InvoiceNumber { get; set; }

        // Link to original PurchaseInvoice
        public Guid OriginalPurchaseInvoiceId { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UGST { get; set; }
        public decimal? SGST { get; set; }

        public string? Narration { get; set; }
        public GstTypes GstType { get; set; }

        public DateTime ReturnDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public PurchaseInvoice PurchaseInvoice { get; set; }
        public PurchaseReturn PurchaseReturn { get; set; }
    }
}
