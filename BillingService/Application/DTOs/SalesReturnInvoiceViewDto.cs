using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entites;

namespace Application.DTOs
{
    public  class SalesReturnInvoiceViewDto
    {

        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        public string ContactNumber { get; set; }
        public string? Email { get; set; }
        public string Place { get; set; }
        public string? GstNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentMode paymentMode { get; set; }   
       
        public decimal TaxableAmount { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TotalCGST { get; set; }
        public decimal TotalSGST { get; set; }
        public decimal TotalIGST { get; set; }
        public decimal TotalUGST { get; set; }
        public decimal TotalTaxAmount =>
           SaleReturnItems?.Sum(i => i.TotalTaxAmount) ?? 0;
        public decimal TotalAmount { get; set; }

        public DateTime ReturnDate { get; set; }
        public Guid returnId { get; set; }
        public List<SalesReturnItemsViewDto> SaleReturnItems { get; set; }
    }
}
