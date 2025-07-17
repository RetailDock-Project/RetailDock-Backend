using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public  class SalesReturnTaxReportDto
    {
public List<SalesReturnViewDto> SalesReturnView { get; set; } 
        public List<HsnReturnTaxReportDto> HsnReturnTaxReport { get; set; }
    }

    public class HsnReturnTaxReportDto 
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
