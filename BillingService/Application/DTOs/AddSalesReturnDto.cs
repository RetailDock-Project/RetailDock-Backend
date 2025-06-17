using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;

namespace Application.DTOs
{
    public class AddSalesReturnDto
    {
        public string  SaleInvoiceNumber { get; set; }

       public  PaymentMode returnPayment { get; set; } = PaymentMode.Cash;
        public string Text { get; set; }

  
        public List<SalesReturnItemsAddDto> ReturnItems { get; set; }

    }
}
