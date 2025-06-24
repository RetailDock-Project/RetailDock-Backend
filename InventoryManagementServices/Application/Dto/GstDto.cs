using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class GstDto
    {
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }

        public decimal SGST { get; set; }
        public decimal UGST { get; set; }
    }
}
