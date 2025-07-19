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

        public Guid ledgerId { get; set; }
        public string CustomerName { get; set; }
        public string Place { get; set; }
        public DateTime SaleDate { get; set; }
        public string PaymentType { get; set; }

        public List<SaleItemsResponseDto> SaleItems { get; set; }
      public decimal TaxableAmount => SaleItems?.Sum(i=>i.TaxableAmount)??0;

        public decimal? TotalCGST => SaleItems?.Sum(i=>i.CGST);
        public decimal? TotalSGST => SaleItems?.Sum(i => i.SGST);
        public decimal? TotalIGST => SaleItems?.Sum(i => i.IGST);
        public decimal? TotalUGST => SaleItems?.Sum(i => i.UGST);
        public decimal? TotalDiscount => SaleItems?.Sum(i => i.DiscountAmount);
        public decimal TotalTaxAmount =>
            SaleItems?.Sum(i => (i.TaxableAmount - i.DiscountAmount) * (i.TaxRate / 100)) ?? 0;

        public decimal TotalAmount => SaleItems?.Sum(i => i.TotalAmount) ?? 0;
    }

}
