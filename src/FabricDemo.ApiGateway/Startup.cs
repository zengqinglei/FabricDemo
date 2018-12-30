using FabricDemo.ApiGateway.Middlewares;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using SkyWalking.AspNetCore;

namespace FabricDemo.ApiGateway
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
            services.AddAuthentication()
                .AddIdentityServerAuthentication("ProductGateway",
                    options =>
                    {
                        options.Authority = _configuration.GetValue<string>("IdentityServer:Authority");
                        options.RequireHttpsMetadata = false;
                        options.ApiName = "productservice-api";
                        options.SupportedTokens = SupportedTokens.Both;
                    });
            services.AddOcelot().AddConsul();

            var directServers = _configuration.GetValue<string>("SkyWalking:DirectServers");
            var applicationCode = _configuration.GetValue<string>("SkyWalking:ApplicationCode");
            if (!string.IsNullOrEmpty(directServers))
            {
                if (string.IsNullOrEmpty(applicationCode))
                {
                    applicationCode = _configuration.GetValue<string>("AuthorizationCenter:AppKey");
                }
                services.AddSkyWalking(options =>
                {
                    options.ApplicationCode = applicationCode;
                    options.DirectServers = directServers;
                });
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRequestLog();

            app.UseOcelot().Wait();
        }
    }
}
