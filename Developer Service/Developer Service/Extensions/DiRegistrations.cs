using Application.Interfaces.IRepository;
using Application.Interfaces.IService;
using Application.Services.OrganizationService;
using Infrastructure.Messaging;
using Infrastructure.Repository.OrganizationRepository;

namespace Developer_Service.Extensions
{
    public static class DiRegistrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationServices, OrganizationService>();
           services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

            return services;
        }
    }
}
