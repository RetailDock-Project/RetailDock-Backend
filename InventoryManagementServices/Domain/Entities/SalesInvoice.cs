using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SalesInvoice
    {
        [Key]

        public Guid  Id { get; set; }

        public string? B2BInvoiceNumber { get; set; }     
        public string? B2CInvoiceNumber { get; set; }
        public Guid OrganisationId { get; set; }

        public decimal TaxableAmount { get; set; } = 0;

        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalCGST { get; set; } = 0;
        public decimal TotalSGST { get; set; } = 0;
        public decimal TotalIGST { get; set; } = 0;
        public decimal TotalUGST { get; set; } = 0;

        public decimal TotalAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
  public Guid UpdatedBy { get;set; }
  public DateTime  UpdatedAt { get; set; }
        public Sales Sales { get; set; }
        //public Users users { get; set; }

}


}   