using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GetVoucherTransactionByVoucherTypeId
    {
        public string VoucherNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public string LedgerName { get; set; }
        public decimal Amount { get; set; }
        public bool IsDebit { get; set; }
        public string Narration { get; set; }
    }
}
