using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public class OrganizationSubscribedEvent
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
