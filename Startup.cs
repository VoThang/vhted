using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using VHTED.Api.Domain;
using VHTED.Api.Domain.Context;
using VHTED.Api.Middleware;
using VHTED.Api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.IdentityModel.Logging;

namespace VHTED.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Env = env;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "logs", "log-{Date}.txt"))
                .CreateLogger();
        }

        public Autofac.IContainer ApplicationContainer { get; private set; }
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();

            services
                .AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Audience = "api";
                    options.Authority = Configuration["OpenId:Endpoint"];
                    options.RequireHttpsMetadata = !Env.IsDevelopment();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role",
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });


            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<VHTEDContext>(options => options.UseSqlServer(Configuration["Connection:VHTED"]));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            var builder = new ContainerBuilder();
            builder.RegisterModule<VHTEDRepositoryModule>();
            builder.RegisterModule<VHTEDServiceModule>();
            builder.RegisterModule<WebApiModule>();

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<VHTEDContext>();
                context.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "logs", "log-{Date}.txt"))
                    .CreateLogger();
                app.UseDeveloperExceptionPage();

            } else if (env.IsProduction() || env.IsStaging())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Error()
                    .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "logs", "log-{Date}.txt"))
                    .CreateLogger();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            loggerFactory.AddSerilog();
            app.UseHandlerGlobalException();

            app.UseCors(
                options => options.WithOrigins(Configuration["Domain:App"].Split(",")).AllowAnyHeader().AllowAnyMethod()
            );
            app.UseAuthentication();
            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }
    }
}
