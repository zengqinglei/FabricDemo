using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using Creekdream.Discovery.Consul;
using FabricDemo.UserService.Middlewares;
using FabricDemo.UserService.Options;

namespace FabricDemo.UserService
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

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new Info { Version = "v1", Title = "用户服务 API 文档" });
                    options.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FabricDemo.UserService.xml"));
                });

            var consulClientConfig = _configuration.GetSection("ConsulClient");
            if (consulClientConfig.Get<ConsulClientOptions>() != null)
            {
                services.AddConsul(consulClientConfig);
            }
            services.Configure<OperationalOptions>(_configuration.GetSection("Operational"));
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

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "用户服务");
                });
            var consulServiceConfig = _configuration.GetSection("ConsulService");
            if (consulServiceConfig.Get<ConsulServiceOptions>() != null)
            {
                app.UseConsul(consulServiceConfig);
            }
            app.UseMvc();
        }
    }
}
