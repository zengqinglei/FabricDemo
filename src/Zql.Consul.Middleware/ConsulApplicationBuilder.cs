using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Zql.Consul.Middleware
{
    /// <inheritdoc />
    public static class ConsulApplicationBuilder
    {
        /// <summary>
        /// 使用服务治理
        /// </summary>
        public static void UseConsul(this IApplicationBuilder app, Action<ConsulServiceOptions> config)
        {
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            var consulServiceOptions = new ConsulServiceOptions();
            config.Invoke(consulServiceOptions);

            var consulClient = new ConsulClient(x => x.Address = consulServiceOptions.ConsulUri);
            var serviceUris = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.Select(p => new Uri(p));

            foreach (var serviceUri in serviceUris)
            {
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = new Uri(serviceUri, consulServiceOptions.ServiceHealthOptions.HealthUrl).AbsoluteUri,
                    Timeout = TimeSpan.FromSeconds(5)
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    ID = Guid.NewGuid().ToString(),
                    Name = consulServiceOptions.ServiceName,
                    Address = serviceUri.Host,
                    Port = serviceUri.Port
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
}
