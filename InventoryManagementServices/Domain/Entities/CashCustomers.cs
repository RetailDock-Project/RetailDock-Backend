using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CashCustomers
    {
        [Key]
       public Guid Id {  get; set; }
        public Guid OrganisationId { get; set; }
        [Required]
        public Guid LedgerId { get; set; }
        public string  CustomerName {get;set;}
        public string? ContactNumber { get;set;}
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
       public Guid  CreatedBy { get;set; }
        public List<Sales> Sales { get; set; }
        //public Users Users { get; set; }
    }
}
