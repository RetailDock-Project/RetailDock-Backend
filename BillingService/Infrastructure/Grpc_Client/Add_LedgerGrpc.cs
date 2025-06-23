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


namespace Infrastructure.Grpc_Client
{
    public class Add_LedgerGrpc : IAddLedger
    {

        private readonly LedgerGrpc.LedgerService.LedgerServiceClient  _ledger;
        public Add_LedgerGrpc( LedgerGrpc.LedgerService.LedgerServiceClient ledger)
        {
            _ledger = ledger;
        }



        public async Task<ResponseDto<object>> AddDebtorLeger(CreateCustomerDto customerData,Guid orgId)
        {
            var request = new LedgerRequest { OrganizationId = orgId.ToString(),AccountNumber=customerData.accountNumber,Address=customerData.Address,BankName=customerData.bankName,ContactName=customerData.contactName,City=customerData.Place,ContactNumber=customerData.PhoneNumber,Country=customerData.Country,GstNumber=customerData.GstNumber,Id=customer.};
           

            var response = _ledger.AddLedger(request);

            return new ResponseDto<object> { Data =response.LedgerId, StatusCode = response.StatusCode };
        }
    }
}
