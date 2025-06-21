//using Application.Interfaces.IServices;
//using AccountingGrpc; // Namespace generated from ledger.proto
//using Grpc.Core;
//using Domain.Entities;

//namespace AccountsServices.Service
//{
//  public class LedgergRPCService : LedgerService.LedgerServiceBase
//    {
//        private readonly ILedgerServices _ledgerServices;
//        private readonly ILogger<LedgergRPCService> _logger;

//        public LedgergRPCService(ILedgerServices ledgerServices, ILogger<LedgergRPCService> logger)
//        {
//            _ledgerServices = ledgerServices;
//            _logger = logger;
//        }
//        public override async Task<LedgerResponse>GetPurchaseLedgers(OrganizationRequest request,ServerCallContext context)
//        {
//            try
//            {
//                var organizationId = Guid.Parse(request.OrganizationId);
//                var result = await _ledgerServices.GetPurchaseAccountLedgerts(organizationId);
//                var response = new LedgerResponse
//                {
//                    StatusCode = result.StatusCode,
//                    Message = result.Message,

//                };
//                if (result.Data != null)
//                {
//                    foreach (var item in result.Data)
//                    {
//                        response.Data.Add(new LedgerDto
//                        {
//                            Id = item.Id.ToString(),
//                            LedgerName = item.LedgerName

//                        });
//                    }
//                }
//                return response;    
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex.Message, "Error occurred while fetching purchase ledgers via gRPC.");
//                return new LedgerResponse
//                {
//                    StatusCode = 500,
//                    Message = "Internal Server Error"
//                };
//            }
//        }
//    }
//}
using Application.DTO;
using Application.Interfaces.IServices;
using Grpc.Core;
using LedgerGrpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class LedgerGrpcService : LedgerService.LedgerServiceBase
{
    private readonly ILedgerServices _ledgerService;
    private readonly ILogger<LedgerGrpcService> _logger;

    public LedgerGrpcService(ILedgerServices ledgerService, ILogger<LedgerGrpcService> logger)
    {
        _ledgerService = ledgerService;
        _logger = logger;
    }

    public override async Task<LedgerResponse> AddLedger(LedgerRequest request, ServerCallContext context)
    {
        try
        {
            var orgId = Guid.Parse(request.OrganizationId);

            var ledgerDTO = new AddLedgerDTO
            {
                LedgerName = request.LedgerName,
                OpeningBalance = Convert.ToDecimal(request.OpeningBalance),
                DrCr = request.DrCr,
                CreatedBy = Guid.Parse(request.CreatedBy),
                UpdateBy = Guid.Parse(request.UpdatedBy),
                Details = new AddLedgerDetailsDTO
                {
                    ContactName = request.ContactName,
                    ContactNumber = request.ContactNumber,
                    Address = request.Address,
                    GSTNumber = request.GstNumber,
                    BankName = request.BankName,
                    AccountNumber = request.AccountNumber,
                    IFSCCode = request.IfscCode,
                    UPIId = request.UpiId
                }
            };

            var result = await _ledgerService.CreateCreditorLedger(ledgerDTO, orgId);

            return new LedgerResponse
            {
                LedgerId = result.Data != Guid.Empty ? result.Data.ToString() : "",
                StatusCode = result.StatusCode,

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC Error in AddLedger");
            return new LedgerResponse
            {
                LedgerId = "",
                StatusCode = 500,

            };
        }
    }
    public override async Task<LedgerResponse> AddDebtor(LedgerRequest request, ServerCallContext context)
    {
        try
        {
            var orgId = Guid.Parse(request.OrganizationId);

            var ledgerDTO = new AddLedgerDTO
            {
                LedgerName = request.LedgerName,
                OpeningBalance = Convert.ToDecimal(request.OpeningBalance),
                DrCr = request.DrCr,
                CreatedBy = Guid.Parse(request.CreatedBy),
                UpdateBy = Guid.Parse(request.UpdatedBy),
                Details = new AddLedgerDetailsDTO
                {
                    ContactName = request.ContactName,
                    ContactNumber = request.ContactNumber,
                    Address = request.Address,
                    GSTNumber = request.GstNumber,
                    BankName = request.BankName,
                    AccountNumber = request.AccountNumber,
                    IFSCCode = request.IfscCode,
                    UPIId = request.UpiId
                }
            };

            var result = await _ledgerService.CreateDebtorLedger(ledgerDTO, orgId);

            return new LedgerResponse
            {
                LedgerId = result.Data != Guid.Empty ? result.Data.ToString() : "",
                StatusCode = result.StatusCode,

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC Error in AddLedger");
            return new LedgerResponse
            {
                LedgerId = "",
                StatusCode = 500,

            };
        }
    }
}
