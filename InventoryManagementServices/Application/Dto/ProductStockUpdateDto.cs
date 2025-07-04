using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ProductStockUpdateDto
    {
        public Guid ProductId { get; set; }
        public Guid OrgId { get; set; }
        public Guid userId { get; set; }
        public int Quantity { get; set; }
        public bool Increase { get; set; } 
     }
}


