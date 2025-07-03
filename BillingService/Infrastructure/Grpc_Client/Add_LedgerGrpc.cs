using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Grpc_Interface;
using Common.ResponseDto;
using Domain.Entites;
using LedgerGrpc;
using Org.BouncyCastle.Bcpg;


namespace Infrastructure.Grpc_Client
{
    public class Add_LedgerGrpc : IAddLedger
    {

        private readonly LedgerGrpc.LedgerService.LedgerServiceClient  _ledger;
        public Add_LedgerGrpc( LedgerGrpc.LedgerService.LedgerServiceClient ledger)
        {
            _ledger = ledger;
        }



        public async Task<ResponseDto<string>> AddDebtor(CreateCustomerDto customerData,Guid orgId,Guid userId)
        {
            var request = new LedgerRequest { OrganizationId = orgId.ToString(),AccountNumber=customerData.accountNumber,Address=customerData.Address,BankName=customerData.bankName,ContactName=customerData.contactName,ContactNumber=customerData.PhoneNumber,GstNumber=customerData.GstNumber,CreatedBy=userId.ToString(),DrCr=customerData.drCr,IfscCode=customerData.ifscCode,OpeningBalance=customerData.openingBalance,LedgerName=customerData.CompanyName,UpiId=customerData.upiId};
           

            var response = _ledger.AddDebtor(request);

            return new ResponseDto<string> { Data =response.LedgerId, StatusCode = response.StatusCode };
        }
    }
}
