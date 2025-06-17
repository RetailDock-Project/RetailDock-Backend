using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs
{
    public class GetAllOrganizatinsDTO
    {
       public string email {  get; set; }
        public Guid SubscriptionId { get; set; } 

        public string SubscriptionName { get; set; }
        public string TransactionId { get; set; }
        
        public Guid OrganizationId { get; set; }


      
      
        public decimal Amount { get; set; } 


        public DateTime CreatedAt { get; set; } 

       
       

      

        public DateTime ExpiryDate { get; set; }



        
        
       
      
        public string OrganizationName { get; set; }

        public Guid UserId { get; set; }//THIS ONLY GET AFTER USERREGISTERD

      
        public string Address { get; set; }

      

        public string LicenceNumber { get; set; }

       
        public string GSTNumber { get; set; }

      
        public string PANNumber { get; set; }

       
        public DateOnly FinancialYearStart { get; set; }

    
        public DateOnly FinancialYearEnd { get; set; }

       

        public bool IsActive { get; set; } = true;

      

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
     
        public DateTime SubscriptionUpdatedAt { get; set; } 
       
    }
}
