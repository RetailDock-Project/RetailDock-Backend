using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using AutoMapper;
using Common.ResponseDto;
using Domain.Entites;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public  class SaleService:ISaleService
    {
        private readonly ISaleRepository saleRepo;
        private readonly ILogger<SaleService> logger;
        private readonly IMapper mapper;
        public SaleService(ISaleRepository _saleRepo,ILogger<SaleService> _logger ,IMapper _mapper)
        {
            ILogger<SaleService> logger = _logger;
            saleRepo = _saleRepo;
            mapper= _mapper;
            
        }

        public async Task<ResponseDto<object>> AddNewSale(SalesAddDto sales, Guid orgId, Guid userId)
        {
            try
            {
                var allIds = new CreateSaleIdsDto
                {
                    InvoiceId = Guid.NewGuid(),
                    OrganisationId = orgId,
                    SaleId = Guid.NewGuid(),
                    UserId = userId
                };

                ResponseDto<object> result;

                if (sales.PaymentType == PaymentMode.Credit)
                {
                    result = await saleRepo.AddNewCreditSale(sales, allIds);

                    // check for any error response from repository
                    if (result.StatusCode != 201)
                    {
                        return result;
                    }
                  

                    await saleRepo.SaveChanges();
                    return result;
                }
                else
                {
                    result = await saleRepo.AddNewCashSale(sales, allIds);

                    if (result.StatusCode != 201)
                    {
                        return result;
                    }
                       

                    await saleRepo.SaveChanges();
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding new sale");
                return new ResponseDto<object> { StatusCode = 500, Message = "Internal Server Error" };
            }
        }

        public async Task<ResponseDto<List<SalesResponseDto>>> GetSalesByDate(DateTime fromDate, DateTime? toDate,Guid orgId)
        {
            try
            {
                DateTime finalToDate = toDate ?? DateTime.Now;

                var sale = await saleRepo.GetSaleDetailsByDate(fromDate, finalToDate,orgId);
                if(sale == null)
                {
                    return new ResponseDto<List<SalesResponseDto>>
                    {
                     
                        Message = "no sales is found between that date",
                        StatusCode = 404
                    };
                }
                var mapped=mapper.Map<List<SalesResponseDto>>(sale);
                return new ResponseDto<List<SalesResponseDto>>
                {
                    Data = mapped,
                    Message = "Sales fetched successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all sales details by date");
                return new ResponseDto<List<SalesResponseDto>>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }


        public async Task<ResponseDto<List<SalesResponseDto>>> GetAllSalesDetails(Guid orgId)
        {
            try
            {
                var totalSales = await saleRepo.GetAllSalesDetails(orgId);
                if (!totalSales.Any())
                {
                    return new ResponseDto<List<SalesResponseDto>>
                    {
                        Message = "Your sales is empty",
                        StatusCode = 200
                    };
                }

                var mapped = mapper.Map<List<SalesResponseDto>>(totalSales);
           
                return new ResponseDto<List<SalesResponseDto>> {Data=mapped, Message = "Fetch All Sales details successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching All sales details ");
                return new ResponseDto<List<SalesResponseDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        public  async   Task<ResponseDto<SalesResponseDto>> GetSalesDetailsById(Guid saleId,Guid orgId)
        {
            
                try
                {
               var sales= await saleRepo.GetSalesDetailsById(saleId,orgId);
                if (sales == null)
                {
                    return new ResponseDto<SalesResponseDto> {  Message = "no such Idsales found", StatusCode = 200 };
                }
                var mapped=mapper.Map<SalesResponseDto>(sales);
                return  new ResponseDto<SalesResponseDto> { Data=mapped, Message = "Fetch Sales details by Id successfully",StatusCode=200 };
                }
                catch (Exception ex)
                {
                logger.LogError(ex, "error from fetching  sales details By Id");

                return new ResponseDto<SalesResponseDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
            
        }
       public async  Task<ResponseDto<SalesResponseDto>> GetSalesDetailsByInvoice(string invoiceNumber,Guid orgId)
        {
            try
            {
                if (invoiceNumber.Contains("B2B"))
                {
                    var sales = await saleRepo.GetB2BSalesDetailsByInvoice(invoiceNumber,orgId);
                    if (sales==null)
                    {
                        return new ResponseDto<SalesResponseDto> { Message = "no such invoiceId Sales found", StatusCode = 200 };
                    }
                    var mapped= mapper.Map<SalesResponseDto>(sales);
                    return new ResponseDto<SalesResponseDto> { Data=mapped,Message = "Fetch B2B Sales details By Invoice successfully", StatusCode = 200 };
                }
                if (invoiceNumber.Contains("B2C"))
                {
                    var sales = await saleRepo.GetB2CSalesDetailsByInvoice(invoiceNumber,orgId);
                    if (sales == null)
                    {
                        return new ResponseDto<SalesResponseDto> { Message = "no such invoiceId Sales found", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesResponseDto>(sales);
                    return new ResponseDto<SalesResponseDto> {Data=mapped, Message = "Fetch B2C Sales details By Invoice successfully", StatusCode = 200 };
                }

                return new ResponseDto<SalesResponseDto> { Message = "no such invoice found", StatusCode = 400 };
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "error from fetching sales details By Invoice");
                return new ResponseDto<SalesResponseDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
    }
}
