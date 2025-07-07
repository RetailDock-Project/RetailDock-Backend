using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GrossProfitDto
    {
        public Guid DirectIncomeGroupId { get; set; }
        public Guid DirectExpenseGroupId { get; set; }
        public decimal? NetDirectIncome { get; set; }
        public decimal? NetDirectExpense { get; set; }
        public decimal? GrossProfit { get; set; }
    }

    public class ProfitAndLossDto
    {
        public Guid IndirectIncomeGroupId { get; set; }
        public Guid IndirectExpenseGroupId { get; set; }
        public decimal? NetIndirectIncome { get; set; }
        public decimal? NetIndirectExpense { get; set; }
    }
    public class PLAccountSummaryDto
    {
        public GrossProfitDto GrossProfitSection { get; set; }

        public ProfitAndLossDto PLAcoountsSection { get; set; }
        public decimal? NetProfit { get; set; }
    }


}
