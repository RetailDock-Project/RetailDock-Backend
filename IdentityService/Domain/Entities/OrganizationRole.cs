using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrganizationRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; } = null;
        public bool IsDeleted { get; set; }=false;
        public ICollection<OrganizationRolePermission> OrganizationRolePermissions { get; set; }
        public ICollection<UserOrganizationRole> UserOrganizationRoles { get; set; }
    }
}
