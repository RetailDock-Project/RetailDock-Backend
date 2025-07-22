
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
using PurchaseGrpc;
using UserGrpc;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagementServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options => {
                options.AddPolicy("Allow", policy => {
                    policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });



            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryService API", Version = "v1" });

                // 1. Bearer token in Authorization header (for Postman, Flutter, etc.)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // 2. Cookie-based token (for browser sessions)
                c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
                {
                    Name = "accessToken",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Cookie,
                    Description = "JWT token stored in the 'accessToken' cookie"
                });

                // Enable both in security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "cookieAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });



            });

            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);



            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // 1. Check Authorization header
                            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                            {
                                context.Token = authHeader["Bearer ".Length..]; // Get token after "Bearer "
                            }

                            // 2. Fallback to cookie if Authorization header is not present
                            if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("accessToken"))
                            {
                                context.Token = context.Request.Cookies["accessToken"];
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false, // set to true and configure if you have issuer
                        ValidateAudience = false, // set to true and configure if you have audience
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero, // no token time leeway
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    };
                });




            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //builder.Services.AddGrpcClient<AccountingGrpc.LedgerService.LedgerServiceClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:7117"); // Update this to correct Inventory Service URL
            //});
            builder.Services.AddGrpcClient<LedgerGrpc.LedgerService.LedgerServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7117"); // Update this to correct Inventory Service URL
            });
            builder.Services.AddGrpcClient<PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceClient>(o =>
            {
                o.Address = new Uri("https://localhost:7117"); // URL of gRPC server
            });

            builder.Services.AddGrpcClient<UserGrpc.UserService.UserServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001"); // gRPC Server URL
            });
            builder.Services.AddScoped<IAccountGrpcService, AccountsGrpcClient>();
            builder.Services.AddScoped<ILedgerGrpcClient, LedgerGrpcClient>();
            builder.Services.AddScoped<IUserGrpcClient, UserGrpcClient>();






            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            builder.Services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IHsnCodeRepository, HsnRepository>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

            builder.Services.AddHttpClient<IOrganizationService, OrganizationApiService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7009/");
            });



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
            //builder.Services.AddScoped<LedgerConsumerService>();



            builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));




            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;



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
            app.UseCors("Allow");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapGrpcService<StockGrpcService>();
            app.MapGrpcService<ProductGrpcService>();
            app.Run();
        }
    }
}
