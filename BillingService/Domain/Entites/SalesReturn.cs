using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class SalesReturn
    {
        [Key] 
        public Guid Id { get; set; }


        public Guid SaleId  {get;set;}

       public Guid OrganisationId {get;set;}
        public Guid ReturnInvoiceId {get;set; }
        public DateTime ReturnDate { get; set; } = DateTime.Now;

       public string  Notes {get;set;}
        public decimal TotalUnitCost { get; set; }
        public Guid  CreatedBy { get; set; }
     

        public Guid? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public  List<SalesReturnItems> SalesReturnItems { get; set; }
        public SalesReturnInvoice ReturnInvoice { get; set; }
        public Sales Sales {  get; set; }   
        //public Users Users { get; set; }

    }
}
