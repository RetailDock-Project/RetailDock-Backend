using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   
        public class LedgerReportDTO
        {
            public DateTime VoucherDate { get; set; }
            public string TypeName { get; set; }
            public string VoucherNumber { get; set; }
            public string OppositeLedger { get; set; }
            public bool IsDebit { get; set; }
            public decimal Amount { get; set; }
        }
        public class LedgerDetailsReportDTO
        {
            public string LedgerName { get; set; }
            public decimal OpeningBalance { get; set; }
           public string OpeningType {  get; set; }
            public decimal ClosingBalance { get; set; }
           public decimal PeriodDr { get; set; }
            public decimal PeriodCr { get; set; }
           public string ClosingType { get; set; } // "Dr" or "Cr"
           public List<LedgerReportDTO> Transactions { get; set; }
        }
    

}

