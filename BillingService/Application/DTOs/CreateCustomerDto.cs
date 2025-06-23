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
        public string CompanyName {  get; set; }   
        public string? Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? GstNumber { get; set; }
    
        public string? Place { get; set; }
        public string? Address { get; set; }



        public string? openingBalance { get; set; } = "";
       public  string? drCr { get; set; } = "dr";

        public string? contactName { get; set; } = "";

        public string? bankName { get; set; }="";
        public string? accountNumber { get; set; }="";
        public string? ifscCode { get; set; }="";
        public string? upiId { get; set; }="";
    }
}
