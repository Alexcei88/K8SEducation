using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.DeliveryService.DAL;
using OTUS.HomeWork.DeliveryService.Extensions;
using Microsoft.Extensions.Options;
using OTUS.HomeWork.RabbitMq.Pool;
using OTUS.HomeWork.RabbitMq;
using OTUS.HomeWork.MessageExchangeSerializer;
using OTUS.HomeWork.DeliveryService.Options;

namespace OTUS.HomeWork.DeliveryService
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
            services.AddDbContext<DeliveryContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

            services.Configure<WarehouseRabbitMQOption>(Configuration.GetSection("RabbitMq"));

            services.AddSingleton(provider => {
                return new MapperConfiguration(cfg =>
                {
                    cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                    cfg.AddProfile(new AutoMapperProfile());
                }).CreateMapper();
            });

            services.AddScoped<Services.DeliveryService>();
            services.AddSingleton((sp) =>
            {
                var rabbitMQOption = sp.GetService<IOptions<WarehouseRabbitMQOption>>()?.Value;
                var chPool = new RabbitMQChannelPool(new RabbitMqConnectionPool(rabbitMQOption.ConnectionString));
                new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                , rabbitMQOption.WarehouseQueueName
                , chPool
                , new JsonNetMessageExchangeSerializer());

                return new RabbitMQMessageSender(rabbitMQOption.ExchangeName
                    , rabbitMQOption.QueueName
                    , chPool
                    , new JsonNetMessageExchangeSerializer());
            });

            services.AddRabbitMQConsumer();

            services.AddControllers();
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.DeliveryService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IMapper mapper, IWebHostEnvironment env)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.DeliveryService v1"));

            AutomaticallyApplyDBMigrations(app);

            app.UseHealthChecks("/api/service/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                AllowCachingResponses = true,
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = "application/json";
                    await c.Response.WriteAsync("{\"status\": \"OK\"}");
                }
            });
            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void AutomaticallyApplyDBMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DeliveryContext>();
            context?.Database.Migrate();
        }
    }
}
