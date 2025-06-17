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
            CreateMap<Sales, SalesResponseDto>().ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.SalesType == "B2B" ? src.Invoices.B2BInvoiceNumber : src.Invoices.B2CInvoiceNumber)).ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.CreatedAt)).ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SaleItems)).ForMember(dest=>dest.CustomerName,opt=>opt.MapFrom(src=>src.DebtorsId.HasValue?src.CreditCustomers.CustomerName:src.CashCustomers.CustomerName));

     
            CreateMap<CashCustomers, ViewCustomerDto>().ForMember(x=>x.CustomerId,opt=>opt.MapFrom(src=>src.Id));
            CreateMap<CreditCustomers, ViewCustomerDto>().ForMember(x => x.CustomerId, opt => opt.MapFrom(src => src.Id)); 

            CreateMap<CashCustomers, ViewCustomerSalesDto>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id)).ForMember(dest=>dest.Sales,opt=>opt.MapFrom(src=>src.Sales));
      
            CreateMap<CreditCustomers, ViewCustomerSalesDto>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.Sales, opt => opt.MapFrom(src => src.Sales));



            CreateMap<SaleItems, SaleItemsResponseDto>();
             

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



            CreateMap<SalesReturn, SalesReturnViewDto>().ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Sales.CashCustomerId.HasValue ? src.Sales.CashCustomers.CustomerName : src.Sales.CreditCustomers.CustomerName)).ForMember(dest => dest.ReturnInvoiceNumber, opt => opt.MapFrom(SRC => SRC.ReturnInvoice.B2CReturnInvoiceNumber??SRC.ReturnInvoice.B2BReturnInvoiceNumber)).ForMember(dest => dest.ReturnId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate)).ForMember(dest => dest.ReturnItems, opt => opt.MapFrom(src => src.SalesReturnItems)).ForMember(dest=>dest.PaymentType,opt=>opt.MapFrom(src=>src.ReturnInvoice.PaymentMode));
            CreateMap<SalesReturnItems, SalesReturnItemsViewDto>();


     
        }

    }
}
 