using System.Net.Http;
using AutoMapper;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Clients.Warehouse;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.EShop.Authentication;
using OTUS.HomeWork.EShop.DAL;
using OTUS.HomeWork.EShop.Middlewares;
using OTUS.HomeWork.EShop.Monitoring;
using OTUS.HomeWork.EShop.Services;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.RestAPI.Abstraction;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Handlers;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Requirements;
using OTUS.HomeWork.RestAPI.Abstraction.DAL;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;
using OTUS.HomeWork.RestAPI.Abstraction.Services;
using Prometheus;

namespace OTUS.HomeWork.EShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrderContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());
            services.AddDbContext<UserContext>(options =>
                   options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

            services.Configure<RabbitMQOption>(Configuration.GetSection("RabbitMq"));

            services.AddSingleton(provider => {
                return new MapperConfiguration(cfg =>
                {
                    cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                    cfg.AddProfile(new AutoMapperProfile());
                }).CreateMapper();
            });

            services.AddHttpClient();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<MetricReporter>();
            services.AddScoped<OrderService>();
            services.AddScoped<UserRepository>();
            services.AddScoped<BucketRepository>();
            services.AddScoped<IUserService, UserService>();

            // TODO лучше вынести в функции расширения
            var billingSettingSection = Configuration.GetSection("BillingService");
            services.AddScoped((sp) =>
            {
                var options = billingSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("BillingClient");
                return new PaymentGatewayClient(options.Url, client);
            });

            var pricingSettingSection = Configuration.GetSection("PricingService");
            services.AddScoped((sp) =>
            {
                var options = pricingSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("PricingService");
                return new PriceServiceClient(options.Url, client);
            });

            var warehouseSettingSection = Configuration.GetSection("WarehouseService");
            services.AddScoped((sp) =>
            {
                var options = warehouseSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("WarehouseService");
                return new WarehouseServiceClient(options.Url, client);
            });

            var deliverySettingSection = Configuration.GetSection("DeliveryService");
            services.AddScoped((sp) =>
            {
                var options = deliverySettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("DeliveryService");
                return new DeliveryServiceClient(options.Url, client);
            });

            services.AddSingleton((sp) =>
            {
                var rabbitMQOption = sp.GetService<IOptions<RabbitMQOption>>()?.Value;
                return new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.QueueName
                    , new RabbitMQChannelPool(new RabbitMqConnectionPool(rabbitMQOption.ConnectionString))
                    , new JsonNetMessageExchangeSerializer());
            });

            var redisOption = Configuration.GetSection("Redis").Get<RedisOption>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisOption.Url;
                options.InstanceName = "Instance-1";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(g =>
            {
                g.DefaultAuthenticateScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
                g.DefaultChallengeScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
                g.DefaultForbidScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
            }).AddScheme<RestAPIAuthOption, SimpleCustomAuthenticationHandler>(SimpleCustomAuthenticationHandler.AuthentificationScheme, o => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyOwner", policy =>
                policy.Requirements.Add(new OwnerPermission()));
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.Eshop", Version = "v1" });
            });

            services.AddProblemDetails();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            AutomaticallyApplyDBMigrations(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.Eshop v1"));

            app.UseHealthChecks("/api/service/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                AllowCachingResponses = true,
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = "application/json";                    
                    await c.Response.WriteAsync("{\"status\": \"OK\"}");
                }
            });

            app.UseMetricServer();

            app.UseProblemDetails();
            app.UseMiddleware<ResponseTimeMiddleware>();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void AutomaticallyApplyDBMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var orderContext = serviceScope.ServiceProvider.GetService<OrderContext>();
            orderContext?.Database.Migrate();

            var userContext = serviceScope.ServiceProvider.GetService<UserContext>();
            userContext?.Database.Migrate();
        }
    }
}
