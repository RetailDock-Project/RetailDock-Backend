
using System.ComponentModel;
using System.Data;
using API.Services;
using Grpc.Net.Client;
using Grpc.Core;
using Application.Interfaces;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Application.Mapper;
using Application.Services;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Messaging;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using QuestPDF.Infrastructure;
using Serilog;
using InventoryService.Services;
using Infrastructure.GrpcClient;

namespace InventoryManagementServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddGrpcClient<AccountingGrpc.LedgerService.LedgerServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7117"); // Update this to correct Inventory Service URL
            });

            builder.Services.AddGrpcClient<VoucherGrpcService.VoucherGrpcServiceClient>(o =>
            {
                o.Address = new Uri("https://localhost:7117"); // URL of gRPC server
            });
            builder.Services.AddScoped<IAccountGrpcService, AccountsGrpcClient>();




            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            builder.Services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IHsnCodeRepository, HsnRepository>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();


            builder.Services.AddScoped<IProductCategoryServices, ProductCategoryServices>();
            builder.Services.AddScoped<IUnitOfMeasureServices, UnitOfMeasureServices>();
            builder.Services.AddScoped<IProductServices, Productservices>();
            builder.Services.AddScoped<IHsnCodeServices, HsnCodeServices>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

            builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IInvoiceNumberGenerator, InvoiceNumberGenerator>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<LedgerConsumerService>();



            builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));




            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "Inventory Service APIs", Version = "v1" });

                // This will ensure Swagger shows enums as strings
                options.UseInlineDefinitionsForEnums();
            });


            // At startup (e.g., Program.cs or Startup.cs), before using EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("My Name or Org");


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)
                )
            );
            Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();


            builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
     });



            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapGrpcService<StockGrpcService>();
            app.MapGrpcService<ProductGrpcService>();
            app.Run();
        }
    }
}
