using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO
{
    
     
        public class AddVouchersDTO
        {

        [Required]
            public Guid VoucherTypeId { get; set; }
        [Required]
        public DateTime VoucherDate { get; set; }
       
        public string? Remarks { get; set; }
        [Required]
        public List<TransactionsDTO> TransactionsDebit { get; set; }
        [Required]
        public List<TransactionsDTO> TransactionsCredit { get; set; }



        }
        public class TransactionsDTO
        {
        [JsonIgnore]

        [Required]
        public Guid VoucherId { get; set; }
        [Required]
        public Guid LedgerId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [JsonIgnore]
        public bool IsDebit { get; set; }=true;
        
        public string? Narration { get; set; }



            

        }
    
}
