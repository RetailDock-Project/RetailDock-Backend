using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs
{
    public class RoleAddDto
    {
        public string Name { get; set; }
    }

    public class RoleDto {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrgRoleDto {
        public Guid? Id { get; set; }
        public Guid OrganizationId { get; set; }
        public int RoleId { get; set; }
    }

    public class GetOrgRoleDto
    {
        public Guid? Id { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
    }

    public class OrgRolePermissionAddDto {
        public Guid? Id { get; set; }
        public Guid OrganizationRoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }

    public class OrgRolePermissionDto
    {
        public Guid? Id { get; set; }
        public Guid OrganizationRoleId { get; set; }
        public int PermissionId { get; set; }
    }

    public class GetOrgRolePermissionDto
    {
        public Guid? Id { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }

    public class UserOrgRole
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationRoleId { get; set; }
    }

    public class PermissionDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }

    }
}
