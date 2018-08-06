using Microsoft.AspNetCore.Mvc;

namespace FabricDemo.IdentityServer.Controllers
{
    /// <summary>
    /// 健康检查服务
    /// </summary>
    public class HealthController : Controller
    {
        /// <summary>
        /// 获取成功状态
        /// </summary>
        [HttpGet]
        public IActionResult Index() => Ok("ok");
    }
}
