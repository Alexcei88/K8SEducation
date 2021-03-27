using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.Common;
using OTUS.HomeWork.RestAPI.Abstraction;
using OTUS.HomeWork.RestAPI.Abstraction.DAL;
using OTUS.HomeWork.RestAPI.Abstraction.Domain;
using OTUS.HomeWork.RestAPI.Abstraction.Services;
using OTUS.HomeWork.Clients;

namespace OTUS.HomeWork.AuthService
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
            services.AddDbContext<UserContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

            services.AddSingleton(provider => {
                return new MapperConfiguration(cfg =>
                {
                    cfg.Advanced.AllowAdditiveTypeMapCreation = true;
                    cfg.AddProfile(new AutoMapperProfile());
                }).CreateMapper();
            });
            
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserRepository>();
            services.AddHttpClient();
            
            var billingSettingSection = Configuration.GetSection("BillingService");
            services.AddScoped<BillingServiceClient>((sp) =>
            {
                var options = billingSettingSection.Get<ServiceAddressOption>();
                var client = sp.GetService<IHttpClientFactory>()?.CreateClient("BillingClient");
                return new BillingServiceClient(options.Url, client);
            });
            
            services.AddControllers();
            
            services.AddHealthChecks();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.AuthService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            mapper.ConfigurationProvider.CompileMappings();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            AutomaticallyApplyDBMigrations(app);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.AuthService v1"));
            
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
        
        private void AutomaticallyApplyDBMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<UserContext>();
            context?.Database.Migrate();
        }
    }
}
