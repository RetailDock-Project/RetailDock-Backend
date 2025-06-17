using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsOrgAdmin { get; set; } = false;   
        public Guid OrganizationId { get; set; }

        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public ICollection<OrganizationRole> OrganizationRoles { get; set; }

    }
}
