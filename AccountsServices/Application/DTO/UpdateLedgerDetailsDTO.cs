using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UpdateLedger
    {
    

        public string? LedgerName { get; set; }
        [JsonIgnore]
        public string Nature {  get; set; }
        public Guid GroupId { get; set; }

        public decimal OpeningBalance { get; set; }
        public string? DrCr { get; set; }
      
        public Guid UpdateBy { get; set; }
        public UpdateLedgerDetailsDTO? Details { get; set; }

    }
    public class UpdateLedgerDetailsDTO
    {
        public string? ContactName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? GSTNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? UPIId { get; set; }





    }
}
