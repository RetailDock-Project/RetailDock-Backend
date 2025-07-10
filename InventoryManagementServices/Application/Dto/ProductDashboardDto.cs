using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ProductDashboardDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }
}
