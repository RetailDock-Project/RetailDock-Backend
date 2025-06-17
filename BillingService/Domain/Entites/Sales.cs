using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class Sales
    {

        [Key]
        public Guid Id { get; set; }    

        public Guid InvoiceId { get; set; }
        public PaymentMode PaymentType { get; set; }        
        public  GST_Type  GST_Type{ get; set; }
        public string SalesType { get; set; } = "B2C";
        public string? Narration { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid? CashCustomerId { get; set; }
        public Guid? DebtorsId { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalUnitCost { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public SalesInvoice Invoices { get; set; }
        public CashCustomers CashCustomers { get; set; }
        public CreditCustomers CreditCustomers { get; set; }
        public SalesReturn SalesReturn { get; set; }
        public List<SaleItems> SaleItems { get; set; }
        //public List<Users> Users { get; set }


    }
    public enum PaymentMode
    {
        Cash, Credit, BankTransfer
    }

public enum GST_Type
{
    SGST,
    UGST,
    IGST
}
}