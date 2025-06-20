using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public  class SaleItems
    {
        [ Key]
      public Guid  Id {  get; set; }
        [Required]

    public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        [Required]
        public decimal  UnitPrice { get; set; } 
        public decimal  UnitCost { get; set; }
        public int HSNCodeNumber { get; set; }

        public decimal TaxableAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UGST { get; set; }
        public decimal TotalAmount {  get; set; }

        public Sales Sales { get; set; }
        public Product Product { get; set; }

        public UnitOfMeasures UnitOfMeasures { get; set; }
    //public List<Product> Products { get; set; }
}
}
