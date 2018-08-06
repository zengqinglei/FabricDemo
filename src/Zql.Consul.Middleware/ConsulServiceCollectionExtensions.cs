using Consul;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zql.Consul.Middleware
{
    /// <inheritdoc />
    public static class ConsulServiceCollectionExtensions
    {
        /// <summary>
        /// 使用服务治理
        /// </summary>
        public static void AddConsul(this IServiceCollection services, Action<ConsulClientConfiguration> config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config));
        }
    }
}
