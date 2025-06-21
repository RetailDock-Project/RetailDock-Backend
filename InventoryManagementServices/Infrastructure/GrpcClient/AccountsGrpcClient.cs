using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.IRepository;
using Domain.Entities;

namespace Infrastructure.GrpcClient
{
    public class AccountsGrpcClient: IAccountGrpcService
    {
        private readonly VoucherGrpcService.VoucherGrpcServiceClient _grpcClient;

        public AccountsGrpcClient(VoucherGrpcService.VoucherGrpcServiceClient grpcClient)
        {
            _grpcClient = grpcClient;

        }

        public async Task<Responses<object>> UpdatePurchaseUccounts(Voucher voucherData)
        {
            try
            {
                var request = new AddVoucherRequest
                {
                    OrganizationId = voucherData.OrganizationId,
                    CreatedBy = voucherData.CreatedBy,
                    VoucherTypeId = voucherData.VoucherTypeId,
                    VoucherDate = voucherData.VoucherDate,
                    Remarks = voucherData.Remarks
                };

                if (voucherData.TransactionsDebit != null)
                {
                    request.TransactionsDebit.AddRange(voucherData.TransactionsDebit.Select(t => new TransactionDTO
                    {
                        LedgerId = t.LedgerId,
                        Amount = t.Amount,
                        Narration = t.Narration,
                    }));
                }

                if (voucherData.TransactionsCredit != null)
                {
                    request.TransactionsCredit.AddRange(voucherData.TransactionsCredit.Select(t => new TransactionDTO
                    {
                        LedgerId = t.LedgerId,
                        Amount = t.Amount,
                        Narration = t.Narration,
                    }));
                }

                var response = await _grpcClient.AddVoucherEntryAsync(request);

                return new Responses<object>
                {
                    StatusCode= response.StatusCode,
                    Message= response.Message,
                    Data= response.Data
                };
            }
            
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calling accounts gRPC service", ex);
            }
        }
    }
}
