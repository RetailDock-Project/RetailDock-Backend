using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public  class SalesReturnInvoice
    {
     
            [Key]

            public Guid Id { get; set; }

        public string? B2BReturnInvoiceNumber { get; set; }
        public string? B2CReturnInvoiceNumber { get; set; }
        [Required]
        public Guid OrganisationId { get; set; }
        [Required]
        public PaymentMode PaymentMode { get; set; }
        public decimal TotalCGST { get; set; } = 0;
        public decimal TotalSGST { get; set; } = 0;
        public decimal TotalIGST { get; set; } = 0;
        public decimal TotalUGST { get; set; } = 0;

        public decimal TaxableAmount { get; set; } = 0;

        [Required]
        public decimal TotalAmount { get; set; } = 0;
        [Required]
        public DateTime CreatedAt { get; set; }  = DateTime.Now;
          public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
     
        
            public SalesReturn SalesReturn { get; set; }
            //public Users users { get; set; }

        
       

    }

}
