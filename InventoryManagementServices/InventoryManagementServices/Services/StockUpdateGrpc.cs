using Application.Dto;
using Application.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using stockUpdate;
namespace InventoryService.Services
{
    public class StockGrpcService : StockService.StockServiceBase
    {
        private readonly ILogger<StockGrpcService> _logger;
        private readonly IProductServices productService;

        public StockGrpcService(ILogger<StockGrpcService> logger, IProductServices _productService)
        {
            _logger = logger;
            productService = _productService;
        }

        public override async Task<updateStockResponse> updateProductStock(updateStockRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Stock update request received");

            // Dummy implementation: Increase or Decrease stock
            if (string.IsNullOrEmpty(request.ProductId))
            {
                return new updateStockResponse
                {
                    Success = false,
                    Message = "Product ID is required"
                };
            }
            var updateData = new ProductStockUpdateDto {
            ProductId=Guid.Parse(request.ProductId),
            OrgId= Guid.Parse(request.OrganisationId),
            userId= Guid.Parse(request.UserId),
            Quantity=request.Quantity,
            Increase=request.Increase,
            };
            var response = await productService.UpdateProductStock(updateData);
            if (response.StatusCode==200) {
                return new updateStockResponse
                {
                    Success = true,
                    Message = "Stock updated"
                };
            }
            return new updateStockResponse
            {
                Success = false,
                Message = "Error in stock update"
            };
        }
    }
}







//﻿using Application.Interfaces.IServices;
//using Application.Services.AccountsService;
//using GroupGrpc; 
//using Grpc.Core;

//namespace AccountsServices.Service
//{
//    public class AccountsGroupgRPCService : AccountsGroupService.AccountsGroupServiceBase
//    {
//        private readonly IAccountsGroupService _accountsGroupService;
//        private readonly ILogger<AccountsGroupgRPCService> _logger;

//        public AccountsGroupgRPCService(IAccountsGroupService accountsGroupService, ILogger<AccountsGroupgRPCService> logger)
//        {
//            _accountsGroupService = accountsGroupService;
//            _logger = logger;
//        }

//        public override async Task<ApiResponse> CreateDefaultGroups(CreateDefaultGroupsRequest request, ServerCallContext context)
//        {
//            try
//            {
//                var organizationId = Guid.Parse(request.OrganizationId);
//                var createdBy = Guid.Parse(request.CreatedBy);



//                var result = await _accountsGroupService.CreateDefaultGroups(organizationId, createdBy);

//                return new ApiResponse
//                {
//                    StatusCode = result.StatusCode,
//                    Message = result.Message ?? string.Empty,
//                    Data = result.Data
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error in CreateDefaultGroups");
//                return new ApiResponse
//                {
//                    StatusCode = 500,
//                    Message = "Internal Server Error",
//                    Data = false
//                };
//            }
//        }
//    }
//}

