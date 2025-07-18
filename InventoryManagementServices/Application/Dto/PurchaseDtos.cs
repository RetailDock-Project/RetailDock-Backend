using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class PurchaseAddDto
    {
        [Required(ErrorMessage = "SupplierId is required.")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "Purchase date is required.")]
        public DateTime Purchasedate { get; set; } = DateTime.UtcNow;

        public Guid? PurchaseOrderId { get; set; }


        public DateTime? DueDate { get; set; }

        //[Required(ErrorMessage = "Payment mode is required.")]
        //public PurchasePaymentMode PaymentMode { get; set; }
        public string? Narration { get; set; }


        //public IFormFile? SupplierInvoice { get; set; }

        [MaxLength(100, ErrorMessage = "Invoice number cannot exceed 100 characters.")]
        public string? SupplierInvoiceNumber { get; set; }
        [Required(ErrorMessage = "GST type items are required.")]

        public GstTypes GstType { get; set; }


        [Required(ErrorMessage = "Purchase items are required.")]
        [JsonPropertyName("purchaseItems")]
        public List<PurchaseItemDto> purchaseItems { get; set; }

        public VoucherDto Voucher { get; set; } 
    }

    public class PurchaseItemDto {


        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Rate per piece is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate per piece must be greater than 0.")]
        public decimal RatePerPiece { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100 percent.")]
        public decimal? Discount { get; set; } = 0;

        public DateOnly? ExpiryDate { get; set; }
    }
    public class GetPurchaseDto
    {
        public Guid Id { get; set; }
        public DateTime Purchasedate { get; set; }
        public decimal TotalAmount { get; set; }
    
        public string? PurchaseOrderNumber { get; set; } = null;
        public string? SupplierInvoiceNumber { get; set; }

        public string PurchaseInvoiceNumber { get; set; }


    }
    public class GetPurchaseDetailsDto
    {
        public Guid PurchaseId { get; set; }

        public DateTime Purchasedate { get; set; }
        public string? PurchaseOrderNumber { get; set; } = null;
        public string? SupplierInvoiceNumber { get; set; }

        public string PurchaseInvoiceNumber { get; set; }
        public SupplierDto SupplierDetails { get; set; }

        public Guid CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public List <PurchaseItemDetailsDto> Items { get; set; }

    }
    public class PurchaseItemDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal RatePerPiece { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UGST { get; set; }
        public decimal? SGST { get; set; }


    }
}
