using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrpcContracts;
using Grpc.Net.Client;
using Grpc.Core;

namespace BillingService.Services
{
    public class ProductConsumerService
    {
        private readonly ProductService.ProductServiceClient _grpcClient;

        public ProductConsumerService(ProductService.ProductServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<List<Product>> GetProductsByOrganizationIdAsync(Guid orgId)
        {
            try
            {
                var request = new OrganizationRequest { OrganizationId = orgId.ToString() };
                var response = await _grpcClient.GetProductsByOrganizationAsync(request);
                return response.Products.ToList();
            }
            catch (RpcException rpcEx)
            {
                // Add more logging or wrap it
                throw new Exception($"gRPC Error: {rpcEx.Status.Detail}", rpcEx);
            }
            catch (Exception ex)
            {
                // Handle other errors
                throw new Exception("An error occurred while calling Inventory gRPC service", ex);
            }
        }
    }
}
