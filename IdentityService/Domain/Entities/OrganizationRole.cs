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
        public Guid OrganizationId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<OrganizationRolePermission> OrganizationRolePermissions { get; set; }
        public ICollection<UserOrganizationRole> UserOrganizationRoles { get; set; }
    }
}
