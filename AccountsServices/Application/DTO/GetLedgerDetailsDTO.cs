﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class GetLedgerDetailsDTO
    {
        public Guid Id { get; set; }
        public string LedgerName { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName {  get; set; }
        public Guid? OrganizationId { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string DrCr { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? GSTNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? UPIId { get; set; }

        public Guid? LedgerDetailsUpdatedBy { get; set; }
        public Guid? LedgerDetailsCreatedBy { get; set; }
        public DateTime? LedgerDetailsCreatedAt { get; set; }
        public DateTime? LedgerDetailsUpdatedAt { get; set; }

    }
}
