using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SalesReturnViewDto
    {

        public Guid ReturnId { get; set; }
        public string ReturnInvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime ReturnDate { get; set; }
        public string PaymentType { get; set; }

        public List<SalesReturnItemsViewDto> ReturnItems { get; set; }

        public decimal TaxableAmount => ReturnItems?.Sum(i => i.TaxableAmount) ?? 0;

        public decimal TotalTaxAmount =>
            ReturnItems?.Sum(i => (i.TaxableAmount) * (i.TaxRate / 100)) ?? 0;

        public decimal TotalAmount => ReturnItems?.Sum(i => i.TotalAmount) ?? 0;
    }
}
