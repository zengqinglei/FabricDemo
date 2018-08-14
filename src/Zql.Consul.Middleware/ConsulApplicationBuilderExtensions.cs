using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace Zql.Consul.Middleware
{
    /// <inheritdoc />
    public static class ConsulApplicationBuilderExtensions
    {
        private static string GetLocalIPAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;

                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    return address.Address.ToString();
                }
            }
            return mostSuitableIp?.Address.ToString();
        }

        /// <summary>
        /// 使用服务治理
        /// </summary>
        public static void UseConsul(this IApplicationBuilder app, Action<ConsulServiceOptions> config)
        {
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            var consulClient = app.ApplicationServices.GetService<IConsulClient>();

            var options = new ConsulServiceOptions();
            config.Invoke(options);

            if (options.ServiceUri == null)
            {
                var addresses = app.ServerFeatures.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First();
                var ip = GetLocalIPAddress();
                if (!string.IsNullOrEmpty(ip))
                {
                    var ipAddress = new Uri(address);
                    options.ServiceUri = new Uri($"{ipAddress.Scheme}://{ip}:{ipAddress.Port}");
                }
            }

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
                Tags = new string[] { },
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
