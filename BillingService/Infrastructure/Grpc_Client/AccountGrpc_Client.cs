using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.Grpc_Interface;
using Application.Interfaces.Repository_Interfaces;
using Common.ResponseDto;
using Domain.Entites;
using PurchaseGrpc;

namespace Infrastructure.Grpc_Client
{
    public class AccountGrpc_Client : IAccountGrpc
    {
        private readonly PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceClient AccountGrpcClient;

        public AccountGrpc_Client(PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceClient _AccountClient)
        {

            AccountGrpcClient = _AccountClient;

        }


        public async Task<ResponseDto<object>> updateSaleAccounts(Voucher voucherData)
        {
            var request = new AddVoucherRequest { CreatedBy = voucherData.CreatedBy, OrganizationId = voucherData.OrganizationId, Remarks = voucherData.Remarks, VoucherDate = voucherData.VoucherDate, VoucherTypeId = voucherData.VoucherTypeId };
            if (voucherData.TransactionsDebit != null)
            {
                request.TransactionsDebit.AddRange(voucherData.TransactionsDebit.Select(dr => new TransactionDTO { Amount = dr.Amount, LedgerId = dr.LedgerId, Narration = dr.Narration }));

            }
            if (voucherData.TransactionsCredit != null)
            {
                request.TransactionsCredit.AddRange(voucherData.TransactionsCredit.Select(cr => new TransactionDTO { Amount = cr.Amount, LedgerId = cr.LedgerId, Narration = cr.Narration }));
            }

            var response = await AccountGrpcClient.AddVoucherEntryAsync(request);
         
            return new ResponseDto<object> { Message = response.Message, StatusCode = response.StatusCode ,Data=response.Data};
        }

    }
}
