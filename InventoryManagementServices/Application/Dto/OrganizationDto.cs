using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class OrganizationDetailsDto
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public string LicenceNumber { get; set; }
        public string GSTNumber { get; set; }
        public string PANNumber { get; set; }
    }
}
