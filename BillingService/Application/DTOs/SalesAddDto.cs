using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entites;
//using PurchaseGrpc;

namespace Application.DTOs
{
    public  class SalesAddDto
    {
        public PaymentMode PaymentType { get; set; }=PaymentMode.Cash;
        public bool creditCustomer { get; set; }=false;
        [Required]
        public string InvoiceNumber { get; set; }
        public string MobileNum { get; set; }
            public string? Text { get; set; }
        public SalesMode SalesMode { get; set; }=SalesMode.B2C;
        public GST_Type GST_Type { get; set; } = GST_Type.SGST;
        public DateTime? DueDate { get; set; }=DateTime.Now.AddDays(30);
        public List<SaleItemsAddDto> SaleItems { get; set; }
        public VoucherDto SaleVoucher { get; set; }
        

    }


    public enum SalesMode {B2B,B2C }

public class CreateSaleIdsDto
{
    public Guid InvoiceId { get; set; }
    public Guid SaleId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganisationId { get; set; }

}
}
