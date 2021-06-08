using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.Clients;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication;
using OTUS.HomeWork.RestAPI.Abstraction.Authentication.Handlers;
using OTUS.HomeWork.WarehouseService.DAL;
using OTUS.HomeWork.WarehouseService.Extensions;
using OTUS.HomeWork.WarehouseService.HangfireJobs;
using OTUS.HomeWork.WarehouseService.Options;
using OTUS.HomeWork.WarehouseService.Services;
using System.Net.Http;

namespace OTUS.HomeWork.WarehouseService
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
            services.AddDbContext<WarehouseContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

            services.Configure<WarehouseRabbitMQOption>(Configuration.GetSection("RabbitMq"));
            services.Configure<RedisOption>(Configuration.GetSection("Redis"));
            services.AddScoped<ProductRepository>();
            services.AddScoped<Services.WarehouseService>();
            services.AddHttpClient();

            var deliveryOptionsSection = Configuration.GetSection("DeliveryService");
            services.AddScoped((sp) =>
            {
                var options = deliveryOptionsSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("DeliveryService");
                return new DeliveryServiceClient(options.Url, client);
            });

            services.AddSingleton(provider => {
                return new MapperConfiguration(cfg =>
                {
                    cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                    cfg.AddProfile(new AutoMapperProfile());
                }).CreateMapper();
            });

            services.AddSingleton((sp) =>
            {
                var rabbitMQOption = sp.GetService<IOptions<WarehouseRabbitMQOption>>()?.Value;
                var chPool = new RabbitMQChannelPool(new RabbitMqConnectionPool(rabbitMQOption.ConnectionString));
                new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.DeliveryRouteKey
                    , chPool
                    , new JsonNetMessageExchangeSerializer());

                new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.WarehouseRouteKey
                    , chPool
                    , new JsonNetMessageExchangeSerializer());

                return new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.EshopNotificationRouteKey
                    , chPool
                    , new JsonNetMessageExchangeSerializer());
            });

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UseMemoryStorage());
            services.AddHangfireServer();

            services.AddRabbitMQConsumer();

            var redisOption = Configuration.GetSection("Redis").Get<RedisOption>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisOption.Url;
                options.InstanceName = "Instance-1";
            });

            services.AddAuthentication(g =>
            {
                g.DefaultAuthenticateScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
                g.DefaultChallengeScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
                g.DefaultForbidScheme = SimpleCustomAuthenticationHandler.AuthentificationScheme;
            }).AddScheme<RestAPIAuthOption, SimpleCustomAuthenticationHandler>(SimpleCustomAuthenticationHandler.AuthentificationScheme, o => { });

            services.Configure<ScheduleJobsOption>(Configuration.GetSection("ScheduleJobs"));
            services.AddTransient<ReserveProductTrackerJob>();
            services.AddHostedService<HangfireJobScheduler>();

            services.AddControllers();
            services.AddHealthChecks(); 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.WarehouseService", Version = "v1" });
            });

            services.AddProblemDetails();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IMapper mapper, IWebHostEnvironment env)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            AutomaticallyApplyDBMigrations(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.WarehouseService v1"));
            }

            app.UseHealthChecks("/api/service/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                AllowCachingResponses = true,
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = "application/json";
                    await c.Response.WriteAsync("{\"status\": \"OK\"}");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.UseProblemDetails();
        }

        private void AutomaticallyApplyDBMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<WarehouseContext>();
            context?.Database.Migrate();
        }
    }
}
