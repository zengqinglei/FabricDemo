using Microsoft.AspNetCore.Mvc;

namespace FabricDemo.UserService.Controllers
{
    /// <summary>
    /// 健康检查服务
    /// </summary>
    public class HealthController : BaseController
    {
        /// <summary>
        /// 获取成功状态
        /// </summary>
        [HttpGet]
        public IActionResult Get() => Ok("ok");
    }
}
