using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class RecentInventoryTransactionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // "Purchase" or "Return"
        public string SupplierOrSource { get; set; }
        public int ItemCount { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }


}
