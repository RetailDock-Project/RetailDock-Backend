using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using AutoMapper;
using Domain.Entites;

namespace Application.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Sales, SalesResponseDto>().ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.SalesType == "B2B" ? src.Invoices.B2BInvoiceNumber : src.Invoices.B2CInvoiceNumber)).ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.CreatedAt))

                
                .ForMember(dest=>dest.SaleItems,opt=>opt.MapFrom(src=>src.SaleItems))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.DebtorsId.HasValue ? src.CreditCustomers.CustomerName : src.CashCustomers.CustomerName)).ForMember(dest=>dest.ledgerId,opt=>opt.MapFrom(src=>src.CashCustomerId.HasValue?src.CashCustomers.LedgerId:src.CreditCustomers.LedgerId));

     
            CreateMap<CashCustomers, ViewCustomerDto>().ForMember(x=>x.CustomerId,opt=>opt.MapFrom(src=>src.Id));
            CreateMap<CreditCustomers, ViewCustomerDto>().ForMember(x => x.CustomerId, opt => opt.MapFrom(src => src.Id)); 

            CreateMap<CashCustomers, ViewCustomerSalesDto>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id)).ForMember(dest=>dest.Sales,opt=>opt.MapFrom(src=>src.Sales));
      
            CreateMap<CreditCustomers, ViewCustomerSalesDto>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.Sales, opt => opt.MapFrom(src => src.Sales));



            CreateMap<SaleItems, SaleItemsResponseDto>()
                .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.Products.ProductName))
                .ForMember(dest=>dest.UnitName,opt=>opt.MapFrom(src=>src.UnitOfMeasures.Measurement));
             

        CreateMap<Sales, ViewCustomerSalesDto>()
                .ForMember(dest => dest.CustomerName,
        opt => opt.MapFrom(src => src.CashCustomerId.HasValue
            ? src.CashCustomers.CustomerName
            : src.CreditCustomers.CustomerName)).ForMember(dest => dest.ContactNumber,
        opt => opt.MapFrom(src => src.CashCustomerId.HasValue
            ? src.CashCustomers.ContactNumber
            : src.CreditCustomers.ContactNumber)).ForMember(dest => dest.CustomerId,
        opt => opt.MapFrom(src => src.CashCustomerId.HasValue
            ? src.CashCustomers.Id
            : src.CreditCustomers.Id)).ForMember(dest => dest.Email,
        opt => opt.MapFrom(src => src.CashCustomerId.HasValue
            ? src.CashCustomers.Email
            : src.CreditCustomers.Email)).ForMember(dest => dest.Place,
        opt => opt.MapFrom(src => src.DebtorsId.HasValue
           ? src.CreditCustomers.Place
            : null)).ForMember(dest => dest.GstNumber,
        opt => opt.MapFrom(src => src.DebtorsId.HasValue
           ? src.CreditCustomers.GstNumber
            : null));



            CreateMap<SalesReturn, SalesReturnViewDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Sales.CashCustomerId.HasValue ? src.Sales.CashCustomers.CustomerName : src.Sales.CreditCustomers.CustomerName)) 


                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Sales.CashCustomerId.HasValue ? src.Sales.CashCustomers.Place : src.Sales.CreditCustomers.Place)) 

                .ForMember(dest => dest.GstNumber, opt => opt.MapFrom(src => src.Sales.CashCustomerId.HasValue ?null : src.Sales.CreditCustomers.GstNumber))

                .ForMember(dest => dest.ReturnInvoiceNumber, opt => opt.MapFrom(SRC => SRC.ReturnInvoice.B2CReturnInvoiceNumber??SRC.ReturnInvoice.B2BReturnInvoiceNumber))

                .ForMember(dest => dest.ReturnId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))

                .ForMember(dest => dest.ReturnItems, opt => opt.MapFrom(src => src.SalesReturnItems))
                .ForMember(dest=>dest.PaymentType,opt=>opt.MapFrom(src=>src.ReturnInvoice.PaymentMode));


            CreateMap<SalesReturnItems, SalesReturnItemsViewDto>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductName)).ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement)); ;




            CreateMap<SalesInvoice, SalesInvoiceViewDto>()
             .ForMember(dest => dest.InvoiceNumber,
                 opt => opt.MapFrom(src => src.B2BInvoiceNumber ?? src.B2CInvoiceNumber))

             .ForMember(dest => dest.Email,
                 opt => opt.MapFrom(src =>
                     src.Sales.CashCustomerId.HasValue
                         ? src.Sales.CashCustomers.Email
                         : src.Sales.CreditCustomers.Email))

             .ForMember(dest => dest.CustomerName,
                 opt => opt.MapFrom(src =>
                     src.Sales.CashCustomerId.HasValue
                         ? src.Sales.CashCustomers.CustomerName
                         : src.Sales.CreditCustomers.CustomerName))

             .ForMember(dest => dest.ContactNumber,
                 opt => opt.MapFrom(src =>
                     src.Sales.CashCustomerId.HasValue
                         ? src.Sales.CashCustomers.ContactNumber
                         : src.Sales.CreditCustomers.ContactNumber))
              .ForMember(dest => dest.GstNumber,
                 opt => opt.MapFrom(src =>
                     src.Sales.CashCustomerId.HasValue
                         ? null
                         : src.Sales.CreditCustomers.GstNumber))

             .ForMember(dest => dest.Place,
                 opt => opt.MapFrom(src =>
                     src.Sales.CashCustomerId.HasValue
                         ? null
                         : src.Sales.CreditCustomers.Place))
             .ForMember(dest => dest.SalesMode,
                 opt => opt.MapFrom(src =>
                     src.Sales.SalesType))
                  

             .ForMember(dest => dest.SaleDate,
                 opt => opt.MapFrom(src => src.Sales.CreatedAt))

             .ForMember(dest => dest.saleId,
                 opt => opt.MapFrom(src => src.Sales.Id))  .ForMember(dest => dest.paymentMod,
                 opt => opt.MapFrom(src => src.Sales.PaymentType))

             .ForMember(dest => dest.pendingAmount,
                 opt => opt.MapFrom(src => src.TotalAmount - src.RecievedAmount))

             .ForMember(dest => dest.SaleItems,
                 opt => opt.MapFrom(src => src.Sales.SaleItems));




            CreateMap<SalesReturnInvoice, SalesReturnInvoiceViewDto>()
                .ForMember(dest => dest.InvoiceNumber,
                    opt => opt.MapFrom(src => src.B2BReturnInvoiceNumber ?? src.B2CReturnInvoiceNumber))
              

                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.SalesReturn.Sales.CashCustomerId.HasValue
                            ? src.SalesReturn.Sales.CashCustomers.CustomerName
                            : src.SalesReturn.Sales.CreditCustomers.CustomerName))

                .ForMember(dest => dest.ContactNumber,
                    opt => opt.MapFrom(src =>
                        src.SalesReturn.Sales.CashCustomerId.HasValue
                            ? src.SalesReturn.Sales.CashCustomers.ContactNumber
                            : src.SalesReturn.Sales.CreditCustomers.ContactNumber))

                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src =>
                        src.SalesReturn.Sales.CashCustomerId.HasValue
                            ? src.SalesReturn.Sales.CashCustomers.Email
                            : src.SalesReturn.Sales.CreditCustomers.Email))

                .ForMember(dest => dest.Place,
                    opt => opt.MapFrom(src =>
                        src.SalesReturn.Sales.CashCustomerId.HasValue
                            ? null
                            : src.SalesReturn.Sales.CreditCustomers.Place))

                .ForMember(dest => dest.GstNumber,
                    opt => opt.MapFrom(src =>
                        src.SalesReturn.Sales.CashCustomerId.HasValue
                            ? null
                            : src.SalesReturn.Sales.CreditCustomers.GstNumber))

                .ForMember(dest => dest.ReturnDate,
                    opt => opt.MapFrom(src => src.SalesReturn.ReturnDate))

                .ForMember(dest => dest.returnId,
                    opt => opt.MapFrom(src => src.SalesReturn.Id))

                .ForMember(dest => dest.SaleReturnItems,
                    opt => opt.MapFrom(src => src.SalesReturn.SalesReturnItems));



        }

    }
}
 