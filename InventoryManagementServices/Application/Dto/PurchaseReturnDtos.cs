using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Dto
{
    public class PurchaseReturnDto
    {

        public Guid OriginalPurchaseId { get; set; }

        public DateOnly ReturnDate { get; set; }
        public Guid SupplierId { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }

        public List<PurchaseReturnItemDto> Items { get; set; }
    }

    public class PurchaseReturnItemDto
    {
        //fr key
        public Guid OriginalPurchaseItemId { get; set; }
        public Guid ProductId { get; set; }
        public int ReturnedQuantity { get; set; }

    }

    public class GetPurchaseReturnDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }

        public string originalInvoiceNumber { get; set; }
        //public string Supplier {  get; set; }
        public DateOnly ReturnDate { get; set; }
        public string Reason { get; set; }

        public int ReturnedQuantity { get; set; }
        public decimal TotalAmount { get; set; }


    }
    public class GetPurchaseReturnDetailsDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }

        public string originalInvoiceNumber { get; set; }
        //public string Supplier {  get; set; }
        public DateOnly ReturnDate { get; set; }

        public DateTime PurchaseDate { get; set; }

        public SupplierDto SupplierDetails { get; set; }
        public int ReturnedQuantity { get; set; }
        public decimal GrossTotalAmount { get; set; }
        public List<PurchaseReturnItemsDetailsDto> PurchaseReturnItemsDetails { get; set; }
    }

    public class PurchaseReturnItemsDetailsDto
    {
        public string ProductName { get; set; }
        public int OriginalQuantity { get; set; }
        public int ReturnedQuantity { get; set; }
        public string Reason { get; set; }
        public decimal TotalAmount { get; set; }
       
    }
}