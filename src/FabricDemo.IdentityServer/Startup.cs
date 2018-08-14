using FabricDemo.IdentityServer.Domain;
using FabricDemo.IdentityServer.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Zql.Consul.Middleware;

namespace FabricDemo.IdentityServer
{
    /// <inheritdoc />
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <inheritdoc />
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionString = _configuration.GetConnectionString("Default");
            var migrationsAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<FabricDemoDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ResolveDbContextOptions =
                        (provider, builder) =>
                        {
                            builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext =
                        builder =>
                        {
                            builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        };
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryCaching();

            services.AddDbContext<FabricDemoDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                });
            services.AddDbContext<ConfigurationDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                });
            services.AddDbContext<PersistedGrantDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                });

            services.AddAuthorization();

            services.AddConsul(
                config =>
                {
                    config.Address = new Uri(_configuration.GetValue<string>("ConsulClient:ClientAddress"));
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            SeedData.Initialize(app.ApplicationServices).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseConsul(
                options =>
                {
                    options.ServiceName = _configuration.GetValue<string>("ConsulService:ServiceName");
                    var serviceAddress = _configuration.GetValue<string>("ConsulService:ServiceAddress");
                    if (!string.IsNullOrEmpty(serviceAddress))
                    {
                        options.ServiceUri = new Uri(serviceAddress);
                    }
                    options.HealthUrl = "health";
                });
            app.UseMvcWithDefaultRoute();
        }
    }
}
