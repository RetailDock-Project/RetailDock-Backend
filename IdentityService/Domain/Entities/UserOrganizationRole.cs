using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserOrganizationRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? OrganizationRoleId { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public User User { get; set; }

        public OrganizationRole OrganizationRole { get; set; }
    }
}
