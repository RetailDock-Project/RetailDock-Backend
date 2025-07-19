using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateCashCustomerDto
    {

        public string CompanyName { get; set; }
        public string? Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? Place { get; set; }
        public Guid  LedgerId { get; set; }

    }
}
