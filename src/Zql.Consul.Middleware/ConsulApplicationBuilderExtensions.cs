using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zql.Consul.Middleware
{
    /// <inheritdoc />
    public static class ConsulApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用服务治理
        /// </summary>
        public static void UseConsul(this IApplicationBuilder app, Action<ConsulServiceOptions> config)
        {
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            var consulClient = app.ApplicationServices.GetService<IConsulClient>();

            var options = new ConsulServiceOptions();
            config.Invoke(options);

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = new Uri(options.ServiceUri, options.HealthUrl).AbsoluteUri,
                Timeout = TimeSpan.FromSeconds(5)
            };

            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = options.ServiceName,
                Address = options.ServiceUri.Host,
                Port = options.ServiceUri.Port
            };

            consulClient.Agent.ServiceRegister(registration).Wait();
            lifetime.ApplicationStopping.Register(
                () =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
        }
    }
}
