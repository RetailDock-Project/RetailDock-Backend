using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SalesReturnItemsViewDto
    {



        public Guid  ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxableAmount => UnitPrice * Quantity;
     
        public decimal TaxRate { get; set; }

        public decimal TotalAmount =>
    TaxableAmount  + (TaxableAmount  * (TaxRate / 100));

    }
}
