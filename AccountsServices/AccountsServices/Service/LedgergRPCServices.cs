using Application.Interfaces.IServices;
using AccountingGrpc; // Namespace generated from ledger.proto
using Grpc.Core;
using Domain.Entities;

namespace AccountsServices.Service
{
  public class LedgergRPCService : LedgerService.LedgerServiceBase
    {
        private readonly ILedgerServices _ledgerServices;
        private readonly ILogger<LedgergRPCService> _logger;

        public LedgergRPCService(ILedgerServices ledgerServices, ILogger<LedgergRPCService> logger)
        {
            _ledgerServices = ledgerServices;
            _logger = logger;
        }
        public override async Task<LedgerResponse>GetPurchaseLedgers(OrganizationRequest request,ServerCallContext context)
        {
            try
            {
                var organizationId = Guid.Parse(request.OrganizationId);
                var result = await _ledgerServices.GetPurchaseAccountLedgerts(organizationId);
                var response = new LedgerResponse
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,

                };
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        response.Data.Add(new LedgerDto
                        {
                            Id = item.Id.ToString(),
                            LedgerName = item.LedgerName
                           
                        });
                    }
                }
                return response;    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error occurred while fetching purchase ledgers via gRPC.");
                return new LedgerResponse
                {
                    StatusCode = 500,
                    Message = "Internal Server Error"
                };
            }
        }
    }
}
