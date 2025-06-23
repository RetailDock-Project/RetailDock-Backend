using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Common;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.AccountsService
{
    public class VoucherService : IVoucherService
    {
        private readonly ILogger<VoucherService> _logger;
        private readonly IAccountsRepository _accountsRepository;
        public VoucherService(ILogger<VoucherService> logger, IAccountsRepository accountsRepository)
        {
            _logger = logger;
            _accountsRepository = accountsRepository;
        }
        public async Task<ApiResponseDTO<bool>> AddVoucherEntrys(Guid organizationId, Guid CreatedBy, AddVouchersDTO addVoucherDTO)
        {
            try
            {
                if (addVoucherDTO == null)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 400,
                        Message = "Enter all details "

                    };

                }
                var allTransactions = new List<TransactionsDTO>();
               

                if (addVoucherDTO.TransactionsDebit != null)
                {
                    foreach (var item in addVoucherDTO.TransactionsDebit)
                    {
                        item.IsDebit = true;
                        allTransactions.Add(item);

                    }
                }
                if (addVoucherDTO.TransactionsCredit != null)
                {
                    foreach (var item in addVoucherDTO.TransactionsCredit)
                    {
                        item.IsDebit = false;
                        allTransactions.Add(item);
                    }
                }

                // Group all transactions by LedgerId and IsDebit
                var groupedTransactions = allTransactions
                    .GroupBy(x => new { x.LedgerId, x.IsDebit })
                    .ToList();

                // Check for duplicate entries on the same side (Dr or Cr)
                var duplicateLedgers = groupedTransactions
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key.LedgerId)
                    .Distinct()
                    .ToList();

                if (duplicateLedgers.Any())
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 400,
                        Message = "A ledger is repeated more than once on the same side (Debit or Credit). Please correct the entry."
                    };
                }

                //  Check for same ledger used on both sides
                var debitLedgerIds = groupedTransactions
                    .Where(g => g.Key.IsDebit)
                    .Select(g => g.Key.LedgerId)
                    .ToHashSet();

                var creditLedgerIds = groupedTransactions
                    .Where(g => !g.Key.IsDebit)
                    .Select(g => g.Key.LedgerId)
                    .ToHashSet();

                var commonLedgers = debitLedgerIds.Intersect(creditLedgerIds).ToList();

                if (commonLedgers.Any())
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 400,
                        Message = "A ledger cannot be used in both Debit and Credit sides. Please correct the entry."
                    };
                }


                var debitSum = allTransactions.Where(x => x.IsDebit).Sum(x => x.Amount);
                var creditSum = allTransactions.Where(x => !x.IsDebit).Sum(x => x.Amount);
                if (debitSum != creditSum)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 400,
                        Message = "Debit and credit amount must be equal"
                    };
                }
                var VoucherNumber = await _accountsRepository.GenerateVoucherNumber(organizationId, addVoucherDTO.VoucherTypeId);
                var voucherId = Guid.NewGuid();

                var voucher = new Vouchers
                {
                    Id = voucherId,
                    VoucherNumber = VoucherNumber,
                    VoucherTypeId=addVoucherDTO.VoucherTypeId,
                    VoucherDate = addVoucherDTO.VoucherDate,
                    OrganizationId = organizationId,
                    Remarks = addVoucherDTO.Remarks,
                    CreatedBy = CreatedBy,
                    CreatedAt = DateTime.Now,


                };
                var result = await _accountsRepository.AddVoucherEntrys(organizationId, CreatedBy, voucher,allTransactions);

                return new ApiResponseDTO<bool>
                {
                    StatusCode = 200,
                    Message = "Transaction Created Succussfully"
                };


            }
            catch (Exception ex)
            {


                _logger.LogError(ex.Message, "Voucher Entry Creation failed");
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 500,
                    Message = "Voucher Entry Creation failed"
                };
            
            }
           


        }
       
    }
}
