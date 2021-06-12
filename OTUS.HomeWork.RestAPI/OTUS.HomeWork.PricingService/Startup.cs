using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.Clients.Warehouse;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.PricingService.Services;
using System.Net.Http;

namespace OTUS.HomeWork.PricingService
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
            services.AddScoped<PriceService>();

            services.AddHttpClient();
            services.AddControllers();
            services.AddHealthChecks();

            services.AddSingleton(provider => {
                return new MapperConfiguration(cfg =>
                {
                    cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                    cfg.AddProfile(new AutoMapperProfile());
                }).CreateMapper();
            });

            var warehouseSettingSection = Configuration.GetSection("WarehouseService");
            services.AddScoped((sp) =>
            {
                var options = warehouseSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("WarehouseService");
                return new WarehouseServiceClient(options.Url, client);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.PricingService", Version = "v1" });
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.PricingService v1"));

            //app.UseHttpsRedirection();

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
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
