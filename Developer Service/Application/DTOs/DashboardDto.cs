using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalOrganizations { get; set; }
        public decimal AmountReceivedThisMonth { get; set; }
        public decimal AmountReceivedThisYear { get; set; }
    }

    public class OrganizationSignupChartDto
    {
        public string Name { get; set; }  // Month name
        public int Users { get; set; }    // Number of organizations signed up
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
    }



}
