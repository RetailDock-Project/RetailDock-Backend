using Application.Dto;
using Application.Services;
using Grpc.Core;
using GrpcContracts;
using Microsoft.AspNetCore.Http;

namespace API.Services
{
    public class ProductGrpcService: ProductService.ProductServiceBase
    {
        private readonly IProductServices _productServices;
        private readonly ILogger<ProductGrpcService> _logger;

        public ProductGrpcService(IProductServices productServices, ILogger<ProductGrpcService> logger)
        {
            _productServices = productServices;
            _logger = logger;
        }

        public override async Task<ProductListResponse> GetProductsByOrganization(OrganizationRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received gRPC request for products of orgId: {OrgId}", request.OrganizationId);

            var result = await _productServices.GetAllProductsForBilling(Guid.Parse(request.OrganizationId));

            if (result.StatusCode != 200)
            {
                throw new RpcException(new Status(StatusCode.NotFound, result.Message));
            }

            var response = new ProductListResponse();

            foreach (var product in result.Data)
            {
                response.Products.Add(new Product
                {
                    Id = product.Id.ToString(),
                    ProductName = product.ProductName,
                    Stock = product.Stock,
                    ProductCategory = product.ProductCategory,
                    TaxRate = (double)product.TaxRate,
                    CostPrice = (double)product.CostPrice,
                    SellingPrice = (double)product.SellingPrice,
                    Mrp = (double)product.MRP,
                    UnitOfMeasures = product.UnitOfMeasures,
                    ProductCode = product.ProductCode
                });

            }

            return response;
        }
    }
}
