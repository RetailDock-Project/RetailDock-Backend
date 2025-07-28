using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   
        public class LedgerReportDTO
        {
        public Guid VoucherId { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public string VoucherTypeDisplay { get; set; }
        public Guid OppositeLedgerId { get; set; }
        public string OppositeLedgerName { get; set; }
        public decimal Amount { get; set; }
        public bool OppositeIsDebit { get; set; }
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

