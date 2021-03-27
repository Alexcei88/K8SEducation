using AutoMapper;
using DataBuffer.BusClient.RabbitMq;
using DataBuffer.BusClient.RabbitMq.Pool;
using DataBuffer.MessageExchangeSerializer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.Eshop.Authentication;
using OTUS.HomeWork.Eshop.DAL;
using OTUS.HomeWork.Eshop.Middlewares;
using OTUS.HomeWork.Eshop.Monitoring;
using OTUS.HomeWork.EShop.DAL;
using OTUS.HomeWork.EShop.Services;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Handlers;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Requirements;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;
using Prometheus;
using System.Net.Http;

namespace OTUS.HomeWork.Eshop
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
            services.AddScoped<ProductRepository>();

            var billingSettingSection = Configuration.GetSection("BillingService");
            services.AddScoped<BillingServiceClient>((sp) =>
            {
                var options = billingSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("BillingClient");
                return new BillingServiceClient(options.Url, client);
            });

            services.AddSingleton<RabbitMQMessageSender>((sp) =>
            {
                var rabbitMQOption = sp.GetService<IOptions<RabbitMQOption>>()?.Value;
                return new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.QueueName
                    , new RabbitMQChannelPool(new RabbitMqConnectionPool(rabbitMQOption.ConnectionString))
                    , new JsonNetMessageExchangeSerializer());
            });
            
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

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.Eshop", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper, ILoggerFactory factory)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            factory.CreateLogger("Startup").LogWarning($"ConnectionString: {Configuration.GetConnectionString("DefaultConnection")}");
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
            
            app.UseMiddleware<ResponseTimeMiddleware>();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private void AutomaticallyApplyDBMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<OrderContext>();
            context?.Database.Migrate();
        }
    }
}
