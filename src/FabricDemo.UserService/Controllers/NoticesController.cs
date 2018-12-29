using FabricDemo.UserService.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace FabricDemo.UserService.Controllers
{
    /// <summary>
    /// 通知服务
    /// </summary>
    public class NoticesController : BaseController
    {
        private readonly ILogger _logger;
        private readonly OperationalOptions _operationalOptions;

        /// <inheritdoc />
        public NoticesController(
            ILogger<NoticesController> logger,
            IOptionsMonitor<OperationalOptions> optionsAccessor)
        {
            _logger = logger;
            _operationalOptions = optionsAccessor.CurrentValue;
        }

        /// <summary>
        /// 运维通知
        /// </summary>
        [HttpPost("ConsulOperational")]
        public async Task ConsulOperational()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                _logger.LogInformation("Consul健康检查通知,发送消息内容：{body}", body);
                var notices = JArray.Parse(body);
                foreach (var notice in notices)
                {
                    var node = notice.Value<string>("Node");
                    var name = notice.Value<string>("Name");
                    var status = notice.Value<string>("Status");
                    _logger.LogWarning(
                        "Consul健康检查通知,请留意 {node} {name} {status}, Consul发送内容为：{content}.",
                        node,
                        name,
                        status,
                        body);

                    // TODO: 发送短信通知

                    // TODO: 发送邮件通知
                }
            }
        }
    }
}
