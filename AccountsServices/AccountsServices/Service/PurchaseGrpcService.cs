using Application.DTO;
using Application.Interfaces.IServices;
using Application.Services.AccountsService;
//using GroupGrpc;
using Grpc.Core;
using Org.BouncyCastle.Asn1.Ocsp;
using PurchaseGrpc;

namespace AccountsServices.Service
{
    public class PurchaseGrpcService : VoucherGrpcService.VoucherGrpcServiceBase
    {
        private readonly IVoucherService _voucherService;
        public PurchaseGrpcService(IVoucherService voucherService)
        {
            _voucherService = voucherService;   
        }
        public override async Task<ApiResponses> AddVoucherEntry(AddVoucherRequest addVoucherRequest,ServerCallContext context)
        {
            var addVoucherDTO= new AddVouchersDTO
            {
                VoucherTypeId=Guid.Parse(addVoucherRequest.VoucherTypeId),
                VoucherDate= DateTime.Parse(addVoucherRequest.VoucherDate),
                Remarks=addVoucherRequest.Remarks,
                TransactionsDebit=addVoucherRequest.TransactionsDebit.Select(x=>new TransactionsDTO
                {
                    LedgerId=Guid.Parse(x.LedgerId),
                    Amount= Convert.ToDecimal(x.Amount),
                   
                    Narration=x.Narration,
                   
                    
                    
                }).ToList(),
                TransactionsCredit = addVoucherRequest.TransactionsCredit.Select(x => new TransactionsDTO
                {
                    LedgerId = Guid.Parse(x.LedgerId),
                    Amount = Convert.ToDecimal(x.Amount),
                    Narration = x.Narration,
                 
                }).ToList()
            };

            var response = await _voucherService.AddVoucherEntrys(Guid.Parse(addVoucherRequest.OrganizationId), Guid.Parse(addVoucherRequest.CreatedBy), addVoucherDTO);

            return new ApiResponses
            {
                StatusCode = response.StatusCode,
                Message = response.Message,
                Data = response.Data
            };
        }
        
    }
}
