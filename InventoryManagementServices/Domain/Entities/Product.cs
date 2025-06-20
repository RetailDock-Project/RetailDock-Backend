using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? OrgnaisationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; }
        public int HsnCodeId { get; set; }

        [Required]
        [Column(TypeName = "MEDIUMBLOB")]
        public byte[] BarCodeImageBase64 { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("Category")]
        public int ProductCategoryId { get; set; }


        public decimal Stock { get; set; } = 0;

        [ForeignKey("UnitOfMeasures")]
        public int UnitOfMeasuresId { get; set; }

        [Range(0, int.MaxValue)]
        public int ReOrderLevel { get; set; }

        public DateTime LastStockUpdate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MRP { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [Range(0, 100)]
       

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public ProductCategory Category { get; set; }
        public HsnCode HsnCode { get; set; }

        public UnitOfMeasures UnitOfMeasures { get; set; }

        public List<Images> Images { get; set; }
        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public List<PurchaseItem> PurchaseItemItems { get;set; }

        public List<PurchaseReturnItem> PurchaseReturnItems { get; set; }

        public SaleItems SaleItems { get; set; }
        public SalesReturnItems SalesReturnItems { get; set; }


    }
}
