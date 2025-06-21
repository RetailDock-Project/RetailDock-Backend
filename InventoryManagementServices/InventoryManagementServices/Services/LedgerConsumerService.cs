//using GrpcContracts;
//using Grpc.Net.Client;
//using Grpc.Core;
//using AccountingGrpc;

//namespace API.Services
//{
//    public class LedgerConsumerService
//    {
//        private readonly LedgerService.LedgerServiceClient ledgerServiceClient;

//        public LedgerConsumerService(LedgerService.LedgerServiceClient _ledgerServiceClient)
//        {
//            ledgerServiceClient = _ledgerServiceClient;
//        }
//        public async Task<LedgerResponse> GetPurchaseLedgers(Guid orgId)
//        {
//            try
//            {
//                var request = new AccountingGrpc.OrganizationRequest { OrganizationId=orgId.ToString() };
//                var response = await ledgerServiceClient.GetPurchaseLedgersAsync(request);
//                return response;
//            }
//            catch (RpcException rpcEx)
//            {
//                // Add more logging or wrap it
//                throw new Exception($"gRPC Error: {rpcEx.Status.Detail}", rpcEx);
//            }
//            catch (Exception ex)
//            {
//                // Handle other errors
//                throw new Exception("An error occurred while calling Ledger gRPC service", ex);
//            }
//        }
//    }
//}
