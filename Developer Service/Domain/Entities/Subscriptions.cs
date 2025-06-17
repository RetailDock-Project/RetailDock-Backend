using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities
{
    public class Subscriptions
    {
        [Key]
        public Guid SubscriptionId { get; set; } = Guid.NewGuid();
       
        public string SubscriptionName { get; set; } = "Base Plan";
        public string TransactionId {  get; set; }
        [Required(ErrorMessage = "OrganizationId is required")]
        public Guid OrganizationId { get; set; }


      
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; } 

        [Required(ErrorMessage = "CreatedAt is required")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "UpdatedAt is required")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "ExpiryDate is required")]
       
        public DateTime ExpiryDate { get; set; }
        public OrganizationDetails OrganizationDetails { get; set; }
    }

    
}

