using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;

namespace Application.DTOs
{
    public class SaleItemsResponseDto
    {

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxableAmount => UnitPrice * Quantity;
        public decimal? DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? SGST {  get; set; } 
        public decimal? CGST {  get; set; } 
        public decimal? IGST {  get; set; } 
        public decimal? UGST {  get; set; } 


        public decimal? TotalAmount=>          
    (TaxableAmount - DiscountAmount) + ((TaxableAmount - DiscountAmount) * (TaxRate / 100));

    }
}
