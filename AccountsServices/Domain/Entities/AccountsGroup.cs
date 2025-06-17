using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ParentAccountsGroup
    {

        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string GroupName { get; set; }
        public Guid AccountsMasterGroupId { get; set; }
        public Guid ParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }


    }
    

}
