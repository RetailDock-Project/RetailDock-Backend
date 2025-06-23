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

        public async Task<Responses<string>> AddSupplierLedger(Supplier newSupplier, decimal? openingBalance, bool? isDebit)
        {
            try
            {
                Console.WriteLine("newSupplier Object:");
                Console.WriteLine($"Id: {newSupplier.Id}");
                Console.WriteLine($"OrganizationId: {newSupplier.OrganizationId}");
                Console.WriteLine($"Name: {newSupplier.Name}");
                Console.WriteLine($"ContactName: {newSupplier.ContactName}");
                Console.WriteLine($"ContactNumber: {newSupplier.ContactNumber}");
                Console.WriteLine($"Address: {newSupplier.Address}");
                Console.WriteLine($"City: {newSupplier.City}");
                Console.WriteLine($"State: {newSupplier.State}");
                Console.WriteLine($"Country: {newSupplier.Country}");
                Console.WriteLine($"Pincode: {newSupplier.Pincode}");
                Console.WriteLine($"GSTNumber: {newSupplier.GSTNumber}");
                Console.WriteLine($"BankName: {newSupplier.BankName}");
                Console.WriteLine($"AccountNumber: {newSupplier.AccountNumber}");
                Console.WriteLine($"IFSCCode: {newSupplier.IFSCCode}");
                Console.WriteLine($"UPIId: {newSupplier.UPIId}");
                Console.WriteLine($"CreatedBy: {newSupplier.CreatedBy}");
                Console.WriteLine($"UpdatedBy: {newSupplier.UpdatedBy}");

                var supplier = new LedgerRequest
                {
                    OrganizationId = newSupplier.OrganizationId.ToString(),
                    LedgerName = newSupplier.Name,
                    OpeningBalance = openingBalance.HasValue ? openingBalance.Value.ToString("F2") : "0.00",
                    DrCr = isDebit.HasValue ? (isDebit.Value ? "Dr" : "Cr") : "Cr", 
                    CreatedBy = newSupplier.CreatedBy != Guid.Empty ? newSupplier.CreatedBy.ToString() : "",
                    UpdatedBy = newSupplier.UpdatedBy != Guid.Empty ? newSupplier.UpdatedBy.ToString() : "",

                    ContactName = newSupplier.ContactName ?? "",
                    ContactNumber = newSupplier.ContactNumber ?? "",
                    Address = newSupplier.Address ?? "",
                    GstNumber = newSupplier.GSTNumber ?? "",
                    BankName = newSupplier.BankName ?? "",
                    AccountNumber = newSupplier.AccountNumber ?? "",
                    IfscCode = newSupplier.IFSCCode ?? "",
                    UpiId = newSupplier.UPIId ?? ""
                };

                var response = await ledgerClient.AddLedgerAsync(supplier);

                return new Responses<string>
                {
                    StatusCode = response.StatusCode,
                    Message = "Ledger added",
                    Data = response.LedgerId
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calling accounts gRPC service", ex);
            }
        }





    }
}
