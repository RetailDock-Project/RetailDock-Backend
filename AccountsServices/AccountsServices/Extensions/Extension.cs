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
<<<<<<< HEAD
//<<<<<<< HEAD
//=======
//            services.AddScoped<IVoucherService, VoucherService>();
//            services.AddScoped<ILedgerReportRepository, LedgerReportRepository>();
//            services.AddScoped<ILedgerReportServices,LedgerReportService>();
//>>>>>>> 286129d61698a8313bd9496190d1e9576be3c255
=======

            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<ILedgerReportRepository, LedgerReportRepository>();
            services.AddScoped<ILedgerReportServices,LedgerReportService>();

>>>>>>> 8f678933a6562d5fdceaf9da092ae77abba5a7bb
            return services;
        }

    }
}
