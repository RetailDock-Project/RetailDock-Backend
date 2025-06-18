using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Supplier
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid LedgerId { get; set; }

        public string Name { get; set; }

        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }
        public string? GSTNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? UPIId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<Purchase> Purchases { get; set; }
        public List<PurchaseReturn> PurchaseReturns { get; set; }
        public List<PurchaseOrder> PurchasesOrder { get; set; }
    }
}
