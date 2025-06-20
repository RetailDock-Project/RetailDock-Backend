using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    
        public class GetLedgerDetailDTO
        {
        public Guid Id { get; set; }            // L.Id AS Id
        public string LedgerName { get; set; }  // L.LedgerName
        public string GroupName { get; set; }   // G.GroupName
        public Guid OrganizationId { get; set; }


    }
}
