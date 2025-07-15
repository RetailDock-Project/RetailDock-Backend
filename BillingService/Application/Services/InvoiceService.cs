using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using AutoMapper;
using Common.ResponseDto;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public  class InvoiceService:I_InvoiceService
    {
        private readonly ILogger<InvoiceService> logger;
        private readonly IMapper mapper;
        private readonly I_InvoiceRepository invoiceRepo;

        public InvoiceService(ILogger<InvoiceService> _logger, IMapper _mapper, I_InvoiceRepository _invoiceRepo)
        {
            logger = _logger;
            mapper = _mapper;
           invoiceRepo = _invoiceRepo;
        }




        public async    Task<ResponseDto<List<SalesInvoiceViewDto>>> getAllSaleInvoices(Guid orgId, Guid userId, bool? isFullData, int? skip, int? take)
        {
            try
            {
                bool fullData = isFullData ?? false;
                var sales = await invoiceRepo.getAllSaleInvoices(orgId, userId, fullData, skip, take);

                if (sales == null)
                {
                    return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "no such invoiceId Salesinvoice found", StatusCode = 200 };
                }
                var mapped = mapper.Map<List<SalesInvoiceViewDto>>(sales);

                return new ResponseDto<List<SalesInvoiceViewDto>> { Data = mapped, Message = "Invoice Details Fetched successfully" ,StatusCode=200};


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching all saleInvoices");
                return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }




        public async Task<ResponseDto<SalesInvoiceViewDto>> getSaleInvoicesByInvoiceNumber(Guid orgId, string invoiceNum)
        {
            try
            {
                if (invoiceNum.Contains("B2B"))
                {
                    var sales= await invoiceRepo.getSaleInvoicesByB2BInvoiceNumber(orgId, invoiceNum);

                    if (sales == null)
                    {
                        return new ResponseDto<SalesInvoiceViewDto> { Message = "no such invoiceId Salesinvoice found", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesInvoiceViewDto>(sales);

                    return new ResponseDto<SalesInvoiceViewDto> { Data = mapped,Message= "Invoice Details Fetched successfully",StatusCode=200 };


                }
                else if (invoiceNum.Contains("B2C"))
                {
                    var sales=await invoiceRepo.getSaleInvoicesByB2CInvoiceNumber(orgId, invoiceNum);

                    if (sales == null)
                    {
                        return new ResponseDto<SalesInvoiceViewDto> { Message = "no such invoiceId Salesinvoice found", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesInvoiceViewDto>(sales);
       

                    return new ResponseDto<SalesInvoiceViewDto> { Data = mapped, Message = "Invoice Details Fetched successfully", StatusCode = 200 };


                }

                return new ResponseDto<SalesInvoiceViewDto> { Message = "no such invoiceNumber", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  saleByInvoices");
                return new ResponseDto<SalesInvoiceViewDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }

      

     public async    Task<ResponseDto<List<SalesInvoiceViewDto>>> getSaleInvoicesByDueDate(Guid orgId, Guid userId, bool? isFullData, DateTime? fromdate, DateTime todate)
        {
            try
            {
                DateTime finalDate = fromdate ?? DateTime.Now; 
                bool fullData = isFullData ?? false;
                var sales = await invoiceRepo.getSaleInvoicesByDueDate(orgId, userId, fullData, finalDate, todate);


                var mapped = mapper.Map<List<SalesInvoiceViewDto>>(sales);

                return new ResponseDto<List<SalesInvoiceViewDto>> { Data = mapped, Message = "Invoice Details Fetched successfully", StatusCode = 200 };


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  saleByDueDate");
                return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }

       public async Task<ResponseDto<List<SalesInvoiceViewDto>>> getPendingSalesInvoices(Guid orgId, Guid userId, bool? isFullData,DateTime? fromDate,DateTime? toDate)
        {
            try
            {
                bool fullData = isFullData ?? false;
                var sales = await invoiceRepo.getPendingSalesInvoices(orgId, userId, fullData,fromDate,toDate);
                if(sales == null)
                {
                    return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "no pending saleInvoices", StatusCode = 200 };
                }
                var mapped = mapper.Map<List<SalesInvoiceViewDto>>(sales);

                return new ResponseDto<List<SalesInvoiceViewDto>> { Data = mapped, Message = "Invoice Details Fetched successfully", StatusCode = 200 };


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  pending saleInvoices");
                return new ResponseDto<List<SalesInvoiceViewDto>>{ Message = "internal Server Error ", StatusCode = 500 };
            }
        }
       public async Task<ResponseDto<List<SalesReturnInvoiceViewDto>>> getAllSaleReturnInvoices(Guid orgId, Guid userId, bool? isFullData, int? skip, int? take)
        {
            try
            {
                bool fullData = isFullData ?? false;
                var saleReturn = await invoiceRepo.getAllSaleReturnInvoices(orgId, userId, fullData, skip, take);


                if(saleReturn == null)
                {
                    return new ResponseDto<List<SalesReturnInvoiceViewDto>> { Message = "no  saleReturnInvoices", StatusCode = 200 };
                }

                var mapped = mapper.Map<List<SalesReturnInvoiceViewDto>>(saleReturn);
                return new ResponseDto<List<SalesReturnInvoiceViewDto>> { Data=mapped, Message = "  saleReturnInvoices fetched successfullyy", StatusCode = 200 };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching All  salereturn ByInvoices");
                return new ResponseDto < List < SalesReturnInvoiceViewDto >> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }   

        public async Task<ResponseDto<List<SalesReturnInvoiceViewDto>>> getSaleReturnInvoicesByDate(Guid orgId, Guid userId,bool? isFullData,DateTime? fromDate , DateTime toDate)
        {
            try
            {
                bool fullData = isFullData ?? false;
                DateTime finalDate = fromDate ?? DateTime.Now;
                var saleReturn = await invoiceRepo.getSaleReturnInvoicesByDate(orgId, userId, fullData,finalDate,toDate);
                if (saleReturn == null)
                {
                    return new ResponseDto<List<SalesReturnInvoiceViewDto>> { Message = "no  saleReturnInvoices", StatusCode = 200 };
                }


                var mapped = mapper.Map<List<SalesReturnInvoiceViewDto>>(saleReturn);
                return new ResponseDto<List<SalesReturnInvoiceViewDto>> { Data = mapped, Message = "  saleReturnInvoices fetched successfullyy", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  get salereturn invoice by date");
                return new ResponseDto < List < SalesReturnInvoiceViewDto >> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }

       public async  Task<ResponseDto<SalesReturnInvoiceViewDto>> getSaleReturnByInvoiceNumber(Guid orgId, string invoiceNum)
        {
            try
            {
                if (invoiceNum.Contains("B2B"))
                {
                    var saleReturn=invoiceRepo.getSaleReturnByB2BInvoiceNumber(orgId, invoiceNum);

                    if (saleReturn == null)
                    {
                        return new ResponseDto<SalesReturnInvoiceViewDto> { Message = "no such invoice saleReturn", StatusCode = 200 };
                    }

                    var mapped = mapper.Map<SalesReturnInvoiceViewDto>(saleReturn);
                    return new ResponseDto<SalesReturnInvoiceViewDto> { Data = mapped, Message = "  saleReturnInvoices fetched successfullyy", StatusCode = 200 };

                }
                else if (invoiceNum.Contains("B2C"))
                {
                    var saleReturn =await  invoiceRepo.getSaleReturnByB2CInvoiceNumber(orgId, invoiceNum);


                    if (saleReturn == null)
                    {
                        return new ResponseDto<SalesReturnInvoiceViewDto> { Message = "no such invoice saleReturn", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesReturnInvoiceViewDto>(saleReturn);
                    return new ResponseDto<SalesReturnInvoiceViewDto> { Data = mapped, Message = "  saleReturnInvoices fetched successfullyy", StatusCode = 200 };
                }
                return new ResponseDto<SalesReturnInvoiceViewDto> {  Message = "  no such invoices", StatusCode = 400 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  sale Return ByInvoice Number");
                return new ResponseDto< SalesReturnInvoiceViewDto > { Message = "internal Server Error ", StatusCode = 500 };
            }
        }



      public async   Task<ResponseDto<List<SalesInvoiceViewDto>>> getSaleInvoicesByDate(Guid orgId, Guid userId, bool? isFullData, DateTime? fromdate, DateTime todate)
        {

            try
            {
                bool fullData = isFullData ?? false;
                DateTime finalDate= fromdate ?? DateTime.Now;
                var sales = await invoiceRepo.getSaleInvoicesByDate(orgId, userId, fullData,finalDate,todate);

                if (sales == null)
                {
                    return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "no  saleInvoices in this Date", StatusCode = 200 };
                }
                var mapped = mapper.Map<List<SalesInvoiceViewDto>>(sales);

                return new ResponseDto<List<SalesInvoiceViewDto>> { Data = mapped, Message = "Invoice Details Fetched successfully", StatusCode = 200 };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error fetching  saleInvoicesBydate ");
                return new ResponseDto<List<SalesInvoiceViewDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }

        }
    }
}
