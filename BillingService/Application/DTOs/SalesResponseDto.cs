using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;

namespace Application.DTOs
{
    public class SalesResponseDto
    {
        public Guid SaleId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public string PaymentType { get; set; }

        public List<SaleItemsResponseDto> Items { get; set; }
      public decimal TaxableAmount =>Items?.Sum(i=>i.TaxableAmount)??0;
        public decimal? TotalCGST =>Items.Sum(i=>i.CGST);
        public decimal? TotalSGST => Items.Sum(i => i.SGST);
        public decimal? TotalIGST => Items.Sum(i => i.IGST);
        public decimal? TotalUGST => Items.Sum(i => i.UGST);
        public decimal? TotalDiscount => Items.Sum(i => i.DiscountAmount);
        public decimal TotalTaxAmount =>
            Items?.Sum(i => (i.TaxableAmount - i.DiscountAmount) * (i.TaxRate / 100)) ?? 0;

        public decimal TotalAmount => Items?.Sum(i => i.TotalAmount) ?? 0;
    }

}
