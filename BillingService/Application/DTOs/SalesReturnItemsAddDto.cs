using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class SalesReturnItemsAddDto
    {

        public Guid ProductId { get; set; }


        public decimal Quantity { get; set; }

    }
}
