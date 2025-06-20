//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Dto
//{
//    public class VoucherDto
//    {
//        public string OrganizationId { get; set; }
//        public string CreatedBy { get; set; }
//        public string VoucherTypeId { get; set; }
//        public string VoucherDate { get; set; }
//        public string Remarks { get; set; }
//        public List<TransactionDto> TransactionsDebit { get; set; }
//        public List<TransactionDto> TransactionsCredit { get; set; }
//    }

//    public class TransactionDto
//    {
//        public string LedgerId { get; set; }
//        public double Amount { get; set; }
//        public string Narration { get; set; }
//        public string VoucherId { get; set; }
//    }
//}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class VoucherDto
    {
        public string VoucherTypeId { get; set; }
        public string VoucherDate { get; set; }
        public string? Remarks { get; set; }
        public List<TransactionDto> TransactionsDebit { get; set; }
        public List<TransactionDto> TransactionsCredit { get; set; }
    }

    public class TransactionDto
    {
        public string LedgerId { get; set; }
        public string Narration { get; set; } = "items purchased";
    }

    public class Voucher
    {
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string VoucherTypeId { get; set; }
        public string VoucherDate { get; set; }
        public string Remarks { get; set; }
        public List<Transaction> TransactionsDebit { get; set; }
        public List<Transaction> TransactionsCredit { get; set; }
    }

    public class Transaction
    {
        public string LedgerId { get; set; }
        public double Amount { get; set; }
        public string Narration { get; set; }
    }
}
