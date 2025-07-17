using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SaleWithTaxReportDto
    {
        public List<SalesResponseDto> SalesDetails { get; set; }
        public List<HsnTaxReportDto> HsnTaxReport { get; set; }
    }

    public class HsnTaxReportDto
    {

        public string HSNCode { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal UGST { get; set; }
        public decimal TotalTaxAmount => CGST + SGST + IGST + UGST;
        public decimal TotalAmount => TotalTaxableAmount + TotalTaxAmount;
    }

}
