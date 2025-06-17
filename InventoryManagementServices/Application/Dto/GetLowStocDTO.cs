using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class GetLowStockDTO
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; }
       
        public string ProductName { get; set; }
       
      
        public int Stock { get; set; }
       
      
        public DateTime LastStockUpdate { get; set; }
       

   
    }
}
