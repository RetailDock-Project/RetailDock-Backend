using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Dto
{
    public class ProductDto
    {

        //public Guid? Id { get; set; }
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public Guid? OrgnaisationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product Category is required.")]
        public int ProductCategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Unit of Measure is required.")]
        public int UnitOfMeasuresId { get; set; }

        [Range(0, int.MaxValue)]
        public int ReOrderLevel { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductCode { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MRP { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        public string CreatedBy { get; set; }
        public int HsnCodeId { get; set; }
        public List<IFormFile> ProductImages { get; set; }
    }
    public class ProductReadDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }


        public string Description { get; set; }
        public string ProductCategory { get; set; }
        public int Stock { get; set; }
        public string UnitOfMeasures { get; set; }
        public int ReOrderLevel { get; set; }
        public DateTime LastStockUpdate { get; set; }
        public decimal MRP { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TaxRate { get; set; }


        public string CreatedBy { get; set; }
        public string BarCodeImageBase64 { get; set; }


        public List<string> ProductImagesBase64 { get; set; }
    }
    public class ProductBillingGetDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Stock { get; set; }
        public decimal TaxRate { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal CostPrice { get; set; }
        public decimal MRP { get; set; }
        public string UnitOfMeasures { get; set; }
        public int UnitOfMeasuresId { get; set; }
        public int HsnCode { get; set; }

        public string ProductCategory { get; set; }
    }
    public class SearchProductDto
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Stock { get; set; }
        public int ReOrderLevel { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
    }

        public class ProductExportDto
        {
            public Guid Id { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }


            public string Description { get; set; }
            public string ProductCategory { get; set; }
            public int Stock { get; set; }
            public string UnitOfMeasures { get; set; }
            public int ReOrderLevel { get; set; }
            public DateTime LastStockUpdate { get; set; }
            public decimal MRP { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal CostPrice { get; set; }
            public decimal TaxRate { get; set; }

        }
    public class ProductStatisticsDto
    {
        public int TotalProducts { get; set; }
        public int LowStockItems { get; set; }
        public decimal InventoryValue { get; set; }
    }
    }

