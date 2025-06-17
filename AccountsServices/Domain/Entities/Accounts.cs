using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VoucherType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
    public class Vouchers
    { 
        public Guid Id { get; set; }
        public string VoucherNumber {  get; set; }
        public Guid VoucherTypeId { get; set; }
        public DateTime VoucherDate { get; set; }
        public Guid OrganizationId { get; set; }
        public string Remarks {  get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
    public class Transactions 
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid VoucherId { get; set; }
        public Guid LedgerId { get; set; }
        public decimal Amount {  get; set; }
        public bool IsDebit {  get; set; }
        public string Narration {  get; set; }
      
     
       
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }



}
