using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class PurchaseInvoice
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }

        public PurchasePaymentMode PaymentMode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Narration { get; set; }
        public GstTypes GstType { get; set; }

        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UGST { get; set; }
        public decimal? SGST { get; set; }
        public bool IsPaid {  get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;

        public Purchase Purchase { get; set; }
        public List<PurchaseReturnInvoice> PurchaseReturnInvoices { get; set; }


    }
}
