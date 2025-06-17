using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        public string SaleMode { get; set; } 

        [Required]
        public string Name {  get; set; }   
        public string? Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? GstNumber { get; set; }
    
        public string? Place { get; set; }


    }
}
