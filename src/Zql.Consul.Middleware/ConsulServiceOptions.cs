using System;

namespace Zql.Consul.Middleware
{
    /// <summary>
    /// 服务健康检查配置
    /// </summary>
    public class ConsulServiceOptions
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// 心跳检查相对地址
        /// </summary>
        public string HealthUrl { get; set; } = "api/health";

        /// <summary>
        /// 检查间隔时间
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 检查超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
