using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;

namespace Application.DTOs
{
    public class SalesInvoiceViewDto
    {
       

        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }
        
        public string ContactNumber { get; set; }
        public string? Email { get; set; }
        public string Place { get; set; }
        public string? GstNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal pendingAmount { get; set; }
        public decimal RecievedAmount { get; set; }
        public decimal TaxableAmount { get; set; } 
        public PaymentMode paymentMod { get; set; }
        public decimal DiscountAmount { get; set; } 
        public decimal TotalCGST { get; set; } 
        public decimal TotalSGST { get; set; } 
        public decimal TotalIGST { get; set; } 
        public decimal TotalUGST { get; set; } 

        public decimal TotalAmount { get; set; } 

        public DateTime SaleDate { get; set; } 
        public Guid saleId { get; set; }
        public List<SaleItemsResponseDto> SaleItems { get; set; }
    }
}
