using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrganizationRolePermission
    {
        public Guid Id { get; set; } 
        public Guid OrganizationRoleId { get; set; }
        public int PermissionId { get; set; }

        public OrganizationRole OrganizationRole { get; set; }
        public Permission Permission { get; set; }


    }
}
