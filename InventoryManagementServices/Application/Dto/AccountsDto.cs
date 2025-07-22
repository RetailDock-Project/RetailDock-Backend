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

    public class InVoiceDetails
    {
        public Guid InvoiceId { get; set; }
        public string invoiceNumber { get; set; }
        public string supplierorcustomerName { get; set; }
        public string salemodeorpurchasemode { get; set; } //B2B
        public decimal BeforeGSTValue { get; set; }
        public decimal GstTotal { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UGST { get; set; }
        public decimal SGST { get; set; }

        public decimal TotalValue { get; set; }

    }
    public class HsnDetails
    {
        public int HsnId { get; set; }
        public string HsnNumber { get; set; }
        public decimal BeforeGSTValue { get; set; }
        public decimal GstTotal { get; set; }
        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UGST { get; set; }
        public decimal SGST { get; set; }

        public decimal TotalValue { get; set; }


    }
    public class InvoiceSummeryDTO
    {

        public int InvoiceCount { get; set; }
        public decimal BeforeGSTValue { get; set; }
        public decimal GstTotal { get; set; }

        public decimal IGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UGST { get; set; }
        public decimal SGST { get; set; }
        public decimal TotalValue { get; set; }

        public List<InVoiceDetails> inVoiceDetails { get; set; }
        public List<HsnDetails> hsnDetails { get; set; }
    }

    public enum InvoiceType { 
        Purchase,
        PurchaseReturn,
        Sales,
        SalesReturn
    }

}
