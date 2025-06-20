using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    
        public class GetLedgerDetailDTO
        {
        public Guid Id { get; set; }          
        public string LedgerName { get; set; }  
        public string GroupName { get; set; }   
        public Guid OrganizationId { get; set; }


    }
}
