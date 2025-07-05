using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrganizationRole
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public Guid? UpdatedBy { get; set; } = null;
        public bool IsDeleted { get; set; }=false;
        public ICollection<OrganizationRolePermission> OrganizationRolePermissions { get; set; }
        public ICollection<UserOrganizationRole> UserOrganizationRoles { get; set; }
    }
}
