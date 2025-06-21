using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AddLedgerDTO
    {
        [Required]
       
        public string LedgerName { get; set; }
        [Required]
        public Guid GroupId { get; set; }
      
        public decimal OpeningBalance { get; set; }
        public string DrCr { get; set; } // "Dr" or "Cr"
        public string Nature { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid UpdateBy { get; set; }
        public AddLedgerDetailsDTO? Details { get; set; }

    }
    public class AddLedgerDetailsDTO
    {
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? GSTNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? UPIId { get; set; }

       
       


    }
    public class NoLeadingOrTrailingSpacesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string str)
            {
                if (str != str.Trim())
                {
                    return new ValidationResult("Ledger name should not contain leading or trailing spaces.");
                }
            }

            return ValidationResult.Success!;
        }
    }

}
