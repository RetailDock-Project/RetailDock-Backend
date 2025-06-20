﻿using System;
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
        public Guid OrganizationRoleId { get; set; }
        public User User { get; set; }
        public OrganizationRole OrganizationRole { get; set; }
    }
}
