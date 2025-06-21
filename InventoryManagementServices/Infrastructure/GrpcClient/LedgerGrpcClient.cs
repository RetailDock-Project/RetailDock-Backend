using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepository;
using Domain.Entities;
using LedgerGrpc;

namespace Infrastructure.GrpcClient
{
    public class LedgerGrpcClient:ILedgerGrpcClient
    {
        private readonly LedgerGrpc.LedgerService.LedgerServiceClient ledgerClient;
        public LedgerGrpcClient(LedgerGrpc.LedgerService.LedgerServiceClient _ledgerClient) { 
        
            ledgerClient = _ledgerClient;
        }

        public async Task<Responses<string>> AddSupplierLedger(Supplier newSupplier) {

            try {
                var supplier = new LedgerRequest
                {
                    Id = newSupplier.Id.ToString(),
                    OrganizationId = newSupplier.OrganizationId.ToString(),
                    LedgerId = newSupplier.LedgerId.ToString(),
                    Name = newSupplier.Name,
                    ContactName = newSupplier.ContactName ?? "",
                    ContactNumber = newSupplier.ContactNumber ?? "",
                    Address = newSupplier.Address ?? "",
                    City = newSupplier.City ?? "",
                    State = newSupplier.State ?? "",
                    Country = newSupplier.Country ?? "",
                    Pincode = newSupplier.Pincode ?? "",
                    GstNumber = newSupplier.GSTNumber ?? "",
                    BankName = newSupplier.BankName ?? "",
                    AccountNumber = newSupplier.AccountNumber ?? "",
                    IfscCode = newSupplier.IFSCCode ?? "",
                    UpiId = newSupplier.UPIId ?? ""
                };

                var response = await ledgerClient.AddLedgerAsync(supplier);
                return new Responses<string> { StatusCode = response.StatusCode, Message = "Ledger added", Data = response.LedgerId };

            } catch (Exception ex)
            {
                throw new Exception("An error occurred while calling accounts gRPC service", ex);
            }

            
    


        }



    }
}
