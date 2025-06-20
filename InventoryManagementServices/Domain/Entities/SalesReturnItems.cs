using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public  class SalesReturnItems
    {
       [Key]
       public Guid Id { get; set; }  
      public Guid ReturnId { get; set; }
       public Guid  ProductId { get; set; }
        public int UnitId { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Quantity { get; set; } = 0;
        public int HSNCodeNumber { get;set; }
        public decimal UnitPrice { get; set; } = 0;
        public decimal TaxableAmount { get; set; } = 0;

        public decimal TaxRate { get; set; }
        public decimal CGST { get; set; } = 0;
        public decimal SGST { get; set; } = 0;
        public decimal IGST { get; set; } = 0;
        public decimal UGST { get; set; } = 0;

        public decimal TotalAmount { get; set; } = 0;
        public SalesReturn SalesReturn { get; set; }

        public Product Products { get; set; }
    }
}
