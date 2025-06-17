using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Application.Services.AccountsService;
using Infrastructure.Repository.AccountsRepository;
using Infrastructure.Repository.GroupRepository;

namespace AccountsService.Extensions
{
    public static class Extension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountsGroupRepository, AccountsGroupRepository>();
            services.AddScoped<IAccountsGroupService, AccountsGroupServices>();
            services .AddScoped<ILedgerRepository, LedgerRepository>();
            services.AddScoped<ILedgerServices, LedgerServices>();
            services.AddScoped<IAccountsRepository, VoucherRepository>();
            services.AddScoped<IVoucherService,VoucherService>();
            return services;
        }

    }
}
