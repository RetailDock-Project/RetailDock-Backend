using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PurchaseItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "PurchaseId is required.")]
        public Guid PurchaseId { get; set; }

        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Rate per piece is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate per piece must be greater than 0.")]
        public decimal RatePerPiece { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100 percent.")]
        public decimal Discount { get; set; } = 0;

        public decimal TaxAmount { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Total amount cannot be negative.")]
        public decimal TotalAmount { get; set; }


        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UGST { get; set; }
        public decimal? SGST { get; set; }

        public DateOnly? ExpiryDate { get; set; }
        public Purchase Purchase { get; set; }
       public Product Product { get; set; }

    }
}
