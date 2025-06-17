using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AddSubscriptionDTO
    {
        [StringLength(255)]
        [Required]
        public string Address { get; set; }

        [StringLength(100)]
        public string OrganizationName { get; set; }
        [StringLength(100)]
        public string LicenceNumber { get; set; }

        [StringLength(15)]
        [MinLength(15)]
        public string GSTNumber { get; set; }

        [StringLength(10)]
        [MinLength(10)]
        [Required]
        public string PANNumber { get; set; }

        [Required]
        public string FinancialYearStart { get; set; }

      
       

    }
}
