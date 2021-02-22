using AutoMapper;
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
using Microsoft.OpenApi.Models;
using OTUS.HomeWork.RestAPI.Authentification;
using OTUS.HomeWork.RestAPI.Authentification.Handlers;
using OTUS.HomeWork.RestAPI.Authentification.Requirements;
using OTUS.HomeWork.RestAPI.Middlewares;
using OTUS.HomeWork.RestAPI.Monitoring;
using OTUS.HomeWork.UserService;
using OTUS.HomeWork.UserService.DAL;
using OTUS.HomeWork.UserService.Domain;
using Prometheus;

namespace OTUS.HomeWork.RestAPI
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
            services.AddScoped<IUserService, UserService.UserService>();
            services.AddScoped<UserRepository>();
            services.AddSingleton<MetricReporter>();

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS.HomeWork.RestAPI", Version = "v1" });
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
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OTUS.HomeWork.RestAPI v1"));
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
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<UserContext>();
                context?.Database.Migrate();
            }
        }
    }
}
