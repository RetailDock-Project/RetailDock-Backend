using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class OrganizationDetails
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrganizationId { get; set; }
        [Required]
        public string OrganizationName { get; set; }
        
        public Guid UserId { get; set; }//THIS ONLY GET AFTER USERREGISTERD

        [StringLength(255)]
        [Required]
        public string Address { get; set; }

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
        public DateOnly FinancialYearStart { get; set; }

        [Required]
        public DateOnly FinancialYearEnd { get; set; }

        [Required]
      
        public bool IsActive { get; set; } = true;

        [Required]
        
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }= DateTime.UtcNow;
        public  Subscriptions Subscriptions { get; set; }
    }


}
