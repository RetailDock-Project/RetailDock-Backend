using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AddReceiptVoucherDTO
    {
      
        [Required]
        public DateTime VoucherDate { get; set; }

        public string? Remarks { get; set; }
        [Required]
        public List<TransactionsDTO> TransactionsDebit { get; set; }
        [Required]
        public List<TransactionsDTO> TransactionsCredit { get; set; }



    }
 
}
