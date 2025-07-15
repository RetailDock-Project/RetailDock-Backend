using Application.Interfaces.Grpc_Interface;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using Application.Services;
using Infrastructure.Grpc_Client;
using Infrastructure.Repositories;

namespace BillingService.Extension
{
    public static class Extension
    {
        public static IServiceCollection addApplicationService(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerRepository, CustomersRepository>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ISaleReturnRepository, SaleReturnRepository>();
            services.AddScoped<ISaleReturnService, SalesReturnService>();
            services.AddScoped<IAddLedger, Add_LedgerGrpc>();
            services.AddScoped<IAccountGrpc, AccountGrpc_Client>();


            services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();


            services.AddScoped<I_InvoiceRepository, InvoiceRepository>();
            services.AddScoped<I_InvoiceService, InvoiceService>();


            return services;
        }
    }
}
