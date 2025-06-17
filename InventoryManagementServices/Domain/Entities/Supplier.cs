using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Supplier
    {


        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }              

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }

        public string? GSTNumber { get; set; }


        public bool IsActive { get; set; } = true;

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<Purchase> Purchases { get; set; }
        public List<PurchaseReturn> PurchaseReturns { get; set; }
        public List<PurchaseOrder> PurchasesOrder { get; set; }
    }
}
