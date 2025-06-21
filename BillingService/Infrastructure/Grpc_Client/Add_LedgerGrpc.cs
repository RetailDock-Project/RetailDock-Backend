//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Application.DTOs;
//using Application.Interfaces.Grpc_Interface;
//using Common.ResponseDto;
//using Domain.Entites;
//using Led

//namespace Infrastructure.Grpc_Client
//{
//    public class Add_LedgerGrpc : IAddLedger
//    {

//        private readonly IAddLedger _ledger;
//        public Add_LedgerGrpc(IAddLedger ledger)
//        {
//            _ledger = ledger;
//        }



//        public async Task<ResponseDto<object>> updateSaleAccounts(CreateCustomerDto voucherData)
//        {
//            var request = new Ledg { CreatedBy = voucherData.CreatedBy, OrganizationId = voucherData.OrganizationId, Remarks = voucherData.Remarks, VoucherDate = voucherData.VoucherDate, VoucherTypeId = voucherData.VoucherTypeId };
//            if (voucherData.TransactionsDebit != null)
//            {
//                request.TransactionsDebit.AddRange(voucherData.TransactionsDebit.Select(dr => new TransactionDTO { Amount = dr.Amount, LedgerId = dr.LedgerId, Narration = dr.Narration }));

//            }
//            if (voucherData.TransactionsCredit != null)
//            {
//                request.TransactionsCredit.AddRange(voucherData.TransactionsCredit.Select(cr => new TransactionDTO { Amount = cr.Amount, LedgerId = cr.LedgerId, Narration = cr.Narration }));
//            }

//            var response = AccountGrpcClient.AddVoucherEntry(request);

//            return new ResponseDto<object> { Message = response.Message, StatusCode = response.StatusCode };
//        }
//    }
//}
