using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using Application.Services;
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

                return services;
        }
    }
}
