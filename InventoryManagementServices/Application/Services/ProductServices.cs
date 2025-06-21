using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Application.Interfaces.IRepository;
using Application.Events;

namespace Application.Services
{
    public interface IProductServices
    {
        Task<Responses<string>> AddProduct(ProductDto productdto);
        Task<Responses<string>> UpdateProduct(Guid id,ProductDto productdto);
        Task<Responses<ProductReadDto>> GetProductById(Guid id);
        Task<Responses<List<ProductReadDto>>> GetAllProducts(Guid OrganizationId);
        Task<Responses<List<ProductBillingGetDto>>> GetAllProductsForBilling(Guid OrganizationId);
        Task <Responses<bool>> DeleteProduct(Guid productId);
        Task<Responses<List<GetLowStockDTO>>> GetLowStockItems(Guid organizationId);
        Task<Responses<List<ProductReadDto>>>GetProductByCategory(int categoryId,Guid OrganizationId);
        Task<Responses<List<SearchProductDto>>> SearchProducts(Guid organizationId, int? categoryId, string searchTerm);

        Task<byte[]> ExportProductsAsExcelAsync(Guid organizationId);
        Task<Responses<ProductStatisticsDto>> GetProductStatistics(Guid organizationId);

        Task<Responses<object>> UpdateProductStock(ProductStockUpdateDto updateData);

        Task<Responses<List<ProductHistoryDTO>>> GetProductHistory(Guid productId);


    }

    public class Productservices:IProductServices
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _repository;
        private readonly ILogger<Productservices>_logger;
        private readonly IRabbitMQProducer _producer;

        public Productservices(IMapper mapper, IProductRepository repository,ILogger <Productservices> logger, IRabbitMQProducer producer)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _producer= producer;
        }

        public async Task<Responses<string>> AddProduct(ProductDto productdto)
        {
            try
            {
                bool productExists = await _repository.ProductExistsAsync(productdto.ProductName, productdto.ProductCode);
                if (productExists)
                {
                    return new Responses<string>
                    {
                        Message = "A product with the same name or code already exists.",
                        StatusCode = 409 
                    };
                }
                else if (productdto.CostPrice >= productdto.SellingPrice)
                {
                    return new Responses<string>
                    {
                        Message = "Cost Price must be less than Selling Price.",
                        StatusCode = 400
                    };
                }

                else if (productdto.SellingPrice >= productdto.MRP)
                {
                    return new Responses<string>
                    {
                        Message = "Selling Price must be less than MRP.",
                        StatusCode = 400
                    };
                }

                else if (productdto.CostPrice >= productdto.MRP)
                {
                    return new Responses<string>
                    {
                        Message = "Cost Price must be less than MRP.",
                        StatusCode = 400
                    };
                }

                var product = _mapper.Map<Product>(productdto);
                product.Id=Guid.NewGuid();
                product.BarCodeImageBase64 = Domain.Entities.BarcodeHelper.GenerateBarcodeBase64(product.ProductCode);
                product.Images = new List<Images>();
                if (productdto.ProductImages != null)
                {
                    foreach (var imageFile in productdto.ProductImages)
                    {
                        var base64 = Domain.Entities.ImageHelper.ConvertToBase64(imageFile);
                        product.Images.Add(new Images { ImageData = base64, FileName=imageFile.FileName,ProductId=product.Id,ContentType=imageFile.GetType().ToString(),CreatedAt=DateTime.UtcNow });
                    }

                }
                //product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;
                await _repository.AddProduct(product);
                var productDetails=await _repository.GetProductById(product.Id);

                var productCreatedEvent = _mapper.Map<ProductCreatedEvent>(productDetails);


                _producer.Publish(productCreatedEvent);

                return new Responses<string> { Message = "Product added succesfully", StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new Responses<string> { Message = ex.Message, StatusCode = 400};

            }
        }
        public async Task<Responses<string>> UpdateProduct(Guid id,ProductDto productdto)
        {
            try
            {
                var prdct= await _repository.GetProductById(id);
                if(prdct == null)
                {
                    return new Responses<string> { Message = "ProductId not found", StatusCode = 400 };
                }

                if (productdto.CostPrice >= productdto.SellingPrice)
                {
                    return new Responses<string>
                    {
                        Message = "Cost Price must be less than Selling Price.",
                        StatusCode = 400
                    };
                }

                if (productdto.SellingPrice >= productdto.MRP)
                {
                    return new Responses<string>
                    {
                        Message = "Selling Price must be less than MRP.",
                        StatusCode = 400
                    };
                }

                if (productdto.CostPrice >= productdto.MRP)
                {
                    return new Responses<string>
                    {
                        Message = "Cost Price must be less than MRP.",
                        StatusCode = 400
                    };
                }

                var product = _mapper.Map(productdto, prdct); 
                product.UpdatedAt = DateTime.UtcNow;
                if (productdto.ProductImages != null)
                {
                    foreach (var imageFile in productdto.ProductImages)
                    {
                        var base64 = Domain.Entities.ImageHelper.ConvertToBase64(imageFile);
                        product.Images.Add(new Images { ImageData = base64, FileName = imageFile.FileName, ProductId = product.Id, ContentType = imageFile.GetType().ToString(), CreatedAt = DateTime.UtcNow });
                    }

                }

                var productUpdatedEvent = _mapper.Map<ProductCreatedEvent>(product);
                _producer.Publish(productUpdatedEvent);


                var data = await _repository.UpdateProduct(product);
                if (data != null)
                {
                    return new Responses<string>
                    {
                        StatusCode = 200,
                        Message = "Product Updated",
                        
                    };
                }
                return new Responses<string>
                {
                    StatusCode = 200,
                    Message = "Product doesn't exist",

                };
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.Message, "Product updated failed");
                return new Responses<string>
                {
                    StatusCode = 500,
                    Message = ex.Message,

                };
            }
        }
        public async Task <Responses<ProductReadDto>> GetProductById(Guid id)
        {
            try
            {
                var product = await _repository.GetProductById(id);
                var productread = _mapper.Map<ProductReadDto>(product);
                if (product == null)
                {
                    return new Responses<ProductReadDto>
                    {
                        StatusCode = 404,
                        Message = "No product is found this id"
                    };
                }
                return new Responses<ProductReadDto>
                {
                    StatusCode = 200,
                    Message = "Product fetched succesfully ",
                    Data = productread
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Product fetched failed");
                return new Responses<ProductReadDto>
                {
                    StatusCode = 500,
                    Message = "Product fetched failed"
                };
            }
        }
        public async Task <Responses<List<ProductReadDto>>> GetAllProducts(Guid OrganizationId)
        {
            try
            {

                var products = await _repository.GetAllProducts(OrganizationId);
                if (products.Count == 0)
                {
                    return new Responses<List<ProductReadDto>>
                    {
                        StatusCode = 200,
                        Message = "No Product found on this organization",

                    };
                }
                var mapped=_mapper.Map<List<ProductReadDto>>(products);
                return new Responses<List<ProductReadDto>>
                {

                    StatusCode = 200,
                    Message = "Product fetched succesfully",
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error while fetching products ");
                return new Responses<List<ProductReadDto>>
                {
                    StatusCode = 500,
                    Message = "Error while fetching products"
                };
            }

        }
        public async Task<Responses<List<ProductBillingGetDto>>> GetAllProductsForBilling(Guid OrganizationId)
        {
            try
            {
                var products = await _repository.GetAllProducts(OrganizationId);
                if (products.Count == 0)
                {
                    return new Responses<List<ProductBillingGetDto>>
                    {
                        Message = "No products Found in this orgnaization",
                        StatusCode = 400
                    };
                }
                var mapped = _mapper.Map<List<ProductBillingGetDto>>(products);
                return new Responses<List<ProductBillingGetDto>>
                {
                    Data = mapped,
                    Message = "Products Fetched",
                    StatusCode = 200
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error while fetchiong products for billing");
                return new Responses<List<ProductBillingGetDto>>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                };

            }
        }
        public async Task<Responses<bool>> DeleteProduct(Guid productId)
        {
            try
            {
                var products = await _repository.DeleteProduct(productId);
                if (products)
                {
                    _producer.Publish(productId);

                    return new Responses<bool>
                    {
                        StatusCode = 200,
                        Message = "Product deleted succesfully",
                        Data= products
                    };
                }
                return new Responses<bool>
                {
                    StatusCode = 404,
                    Message = "Products not found"
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in product deleting", ex.Message);
                return new Responses<bool>
                {
                    StatusCode = 500,
                    Message = "Error in product deleting"
                };
            }
        }
        public async Task<Responses<List<GetLowStockDTO>>> GetLowStockItems(Guid organizationId)
        {
            try
            {      
                var products = await _repository.GetLowStock(organizationId);
                var stocks = _mapper.Map<List<GetLowStockDTO>>(products);
                if (stocks.Count<=0)
                { 
                  return new Responses<List<GetLowStockDTO>>
                  {
                    StatusCode = 200,
                    Message = "No Low stock items",
                  };
                }
                return new Responses<List<GetLowStockDTO>>
               {
                    StatusCode = 200,
                    Message = " Low stock items fetched succesfully",
                    Data = stocks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error while getting low stock items");
                return new Responses<List<GetLowStockDTO>>
                {

                    StatusCode = 500,
                    Message = "Error while getting low stock items"
                };

            }
         }
        public async Task<Responses<List<ProductReadDto>>> GetProductByCategory(int categoryId, Guid OrganizationId)
        {
            try { 
            var products = await _repository.GetProductByCategory(categoryId, OrganizationId);
            var productByCategory = _mapper.Map<List<ProductReadDto>>(products);
            if (productByCategory.Count <= 0)
            {
                return new Responses<List<ProductReadDto>>
                {
                    StatusCode = 200,
                    Message = "No product found this category or organization"
                };

            }
            return new Responses<List<ProductReadDto>>
            {
                StatusCode = 200,
                Message = "Product by category fetched succesfully",
                Data = productByCategory

            };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Error while getting product by category ");
                return new Responses<List<ProductReadDto>>
                {
                    StatusCode = 500,
                    Message = "Error while getting product by category ",

                };
            }
        }
        public async Task<Responses<List<SearchProductDto>>> SearchProducts(Guid organizationId, int? categoryId, string searchTerm)
        {
            try
            {


                var searchitems = await _repository.SearchProducts(organizationId, categoryId, searchTerm);
                var searchProduct = _mapper.Map<List<SearchProductDto>>(searchitems);
                if (searchitems.Count <= 0)
                {
                    return new Responses<List<SearchProductDto>>
                    {
                        StatusCode = 200,
                        Message = "No item by this keyword"
                    };
                }
                return new Responses<List<SearchProductDto>>
                {
                    StatusCode = 200,
                    Message = "Getting product using keyword is succesfully",
                    Data = searchProduct
                };
            }
            catch(Exception ex)
            { _logger.LogError(ex.Message,"Error while getting product by keyword");
                return new Responses<List<SearchProductDto>>
                {
                    StatusCode = 500,
                    Message = "Error while getting product by keyword"
                };
            }
        }

        public async Task<byte[]> ExportProductsAsExcelAsync(Guid organizationId)
        {
            var products = await _repository.GetAllProducts(organizationId);
            var result = _mapper.Map<List<ProductExportDto>>(products);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Products");

            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Product Name";
            worksheet.Cells[1, 3].Value = "Product Code";
            worksheet.Cells[1, 4].Value = "Description";
            worksheet.Cells[1, 5].Value = "Category";
            worksheet.Cells[1, 6].Value = "Stock";
            worksheet.Cells[1, 7].Value = "Unit of Measures";
            worksheet.Cells[1, 8].Value = "Reorder Level";
            worksheet.Cells[1, 9].Value = "Last Stock Update";
            worksheet.Cells[1, 10].Value = "MRP";
            worksheet.Cells[1, 11].Value = "Selling Price";
            worksheet.Cells[1, 12].Value = "Cost Price";
            worksheet.Cells[1, 13].Value = "Tax Rate";

            int row = 2;
            foreach (var product in result)
            {
                worksheet.Cells[row, 1].Value = product.Id.ToString();
                worksheet.Cells[row, 2].Value = product.ProductName;
                worksheet.Cells[row, 3].Value = product.ProductCode;
                worksheet.Cells[row, 4].Value = product.Description;
                worksheet.Cells[row, 5].Value = product.ProductCategory;
                worksheet.Cells[row, 6].Value = product.Stock;
                worksheet.Cells[row, 7].Value = product.UnitOfMeasures;
                worksheet.Cells[row, 8].Value = product.ReOrderLevel;
                worksheet.Cells[row, 9].Value = product.LastStockUpdate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 10].Value = product.MRP;
                worksheet.Cells[row, 11].Value = product.SellingPrice;
                worksheet.Cells[row, 12].Value = product.CostPrice;
                worksheet.Cells[row, 13].Value = product.TaxRate;

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<Responses<ProductStatisticsDto>> GetProductStatistics(Guid organizationId)
        {
            try
            {
                var result = await _repository.GetProductStatisticsAsync(organizationId);

                return new Responses<ProductStatisticsDto>
                {
                    StatusCode = 200,
                    Message = "Product statistics retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving product statistics");
                return new Responses<ProductStatisticsDto>
                {
                    StatusCode = 500,
                    Message = "Error while retrieving product statistics"
                };
            }
        }


        public async Task<Responses<object>> UpdateProductStock(ProductStockUpdateDto updateData)
        {
            try {

                var result = await _repository.ProductStockUpdate(updateData);
                if (!result) {
                    return new Responses<object>
                    {
                        StatusCode = 404,
                        Message = "Product not found",
                    };
                }
                return new Responses<object>
                {
                    StatusCode = 200,
                    Message = "Product Updated",
                };
            } catch (Exception ex) {

                return new Responses<object>
                {
                    StatusCode = 500,
                    Message = "Error in  Updating product",
                };
            }
        }

        public async Task<Responses<List<ProductHistoryDTO>>> GetProductHistory(Guid productId) {
            try
            {

                if (productId == null || Guid.Empty == productId)
                {
                    return new Responses<List<ProductHistoryDTO>>
                    {
                        StatusCode = 404,
                        Message = "Product not found",
                    };
                }

                var product = await _repository.GetProductHistory(productId);
                var history = new List<ProductHistoryDTO>();

                // Sale Items
                if (product.SaleItems != null)
                {
                    history.Add(new ProductHistoryDTO
                    {
                        Date = product.SaleItems.Sales.CreatedAt,
                        Type = "Sale",
                        Quantity = Convert.ToDecimal(product.SaleItems.Quantity),
                        ReferenceNumber = product.SaleItems.Sales.Invoices.B2CInvoiceNumber ?? product.SaleItems.Sales.Invoices.B2BInvoiceNumber
                    });
                }

                // Sales Return Items
                if (product.SalesReturnItems != null)
                {
                    history.Add(new ProductHistoryDTO
                    {
                        Date = product.SalesReturnItems.SalesReturn.ReturnDate,
                        Type = "Sales Return",
                        Quantity = Convert.ToDecimal(product.SalesReturnItems.Quantity),
                        ReferenceNumber = product.SalesReturnItems.SalesReturn.ReturnInvoice.B2CReturnInvoiceNumber ?? product.SalesReturnItems.SalesReturn.ReturnInvoice.B2BReturnInvoiceNumber
                    });
                }

                // Purchase Items
                if (product.PurchaseItems != null && product.PurchaseItems.Any())
                {
                    foreach (var pi in product.PurchaseItems)
                    {
                        history.Add(new ProductHistoryDTO
                        {
                            Date = pi.Purchase.CreatedAt,
                            Type = "Purchase",
                            Quantity = Convert.ToDecimal(pi.Quantity),
                            ReferenceNumber = pi.Purchase.PurchaseInvoice.InvoiceNumber
                        });
                    }
                }

                // Purchase Return Items
                if (product.PurchaseReturnItems != null && product.PurchaseReturnItems.Any())
                {
                    foreach (var pri in product.PurchaseReturnItems)
                    {
                        history.Add(new ProductHistoryDTO
                        {
                            Date = pri.PurchaseReturn.CreatedAt,
                            Type = "Purchase Return",
                            Quantity = Convert.ToDecimal(pri.ReturnedQuantity),
                            ReferenceNumber = pri.PurchaseReturn.PurchaseReturnInvoice.InvoiceNumber
                        });
                    }
                }
                return new Responses<List<ProductHistoryDTO>> { StatusCode = 200 ,Message="Product history retrieved successfully",Data= history.OrderByDescending(h => h.Date).ToList() };
                // Sort history by date
                

            }
            catch (Exception ex)
            {
                return new Responses<List<ProductHistoryDTO>> { StatusCode = 500, Message = "Error in retrieving Product history retrieved successfully" };

            }
        }
    }
}
