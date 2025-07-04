using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class LedgerSummaryDTO
    {
        public Guid LedgerId { get; set; }
        public string LedgerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public string OpeningType { get; set; }
        public decimal PeriodDr { get; set; }
        public decimal PeriodCr { get; set; }
        public decimal ClosingBalance { get; set; }
        public string ClosingType { get; set; }
    }


    public class GroupLedgerSummaryDTO
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string Nature { get; set; }
        public decimal OpeningBalance { get; set; }
        public string OpeningType { get; set; }
        public decimal PeriodDr { get; set; }
        public decimal PeriodCr { get; set; }
        public decimal ClosingBalance { get; set; }
        public string ClosingType { get; set; }
    }

    public class DirectLedgerSummaryDTO
    {
        public Guid LedgerId { get; set; }
        public string LedgerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public string OpeningType { get; set; }
        public decimal PeriodDr { get; set; }
        public decimal PeriodCr { get; set; }
        public decimal ClosingBalance { get; set; }
        public string ClosingType { get; set; }
    }

    public class GroupWithLedgersSummaryDTO
    {
        public List<GroupLedgerSummaryDTO> GroupSummaries { get; set; }
        public List<DirectLedgerSummaryDTO> DirectLedgerSummaries { get; set; }
    }
}
