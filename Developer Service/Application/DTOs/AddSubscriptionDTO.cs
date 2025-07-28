using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class AddSubscriptionDTO
    {
        [StringLength(255)]
        [Required]
        public string Address { get; set; }

        [StringLength(100)]
        public string OrganizationName { get; set; }
        [StringLength(100)]
        public string LicenceNumber { get; set; }

        [StringLength(15)]
        [MinLength(15)]
        public string GSTNumber { get; set; }

        [StringLength(10)]
        [MinLength(10)]
        [Required]
        public string PANNumber { get; set; }

        [Required]
        public string FinancialYearStart { get; set; }

      
       

    }


    public class OrganizationResponseDTO
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string LicenceNumber { get; set; }
        public string GstNumber { get; set; }
        public string PanNumber { get; set; }
        public string FinancialYearStart { get; set; }
        public string FinancialYearEnd { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SubscriptionDto Subscriptions { get; set; }
    }

    public class SubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

}
