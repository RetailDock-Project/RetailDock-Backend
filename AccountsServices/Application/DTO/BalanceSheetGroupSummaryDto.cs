using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class BalanceSheetGroupSummaryDto
    {
        public Guid ImmediateGroupId { get; set; }
        public string GroupName { get; set; }
        public string Side { get; set; } // "Asset" or "Liability"
        public decimal? NetAmount { get; set; }
    }

    public class BalanceSheetResponseDto
    {
        public List<BalanceSheetGroupSummaryDto> Items { get; set; } = new();
        public decimal? TotalAssets { get; set; }
        public decimal? TotalLiabilities { get; set; }
        public decimal? NetProfit {  get; set; }
    }

}
