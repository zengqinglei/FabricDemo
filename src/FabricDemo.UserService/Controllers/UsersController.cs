using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace FabricDemo.UserService.Controllers
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UsersController : BaseController
    {
        private readonly IConfiguration _configuration;

        /// <inheritdoc />
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 用户服务输出示例
        /// </summary>
        [HttpGet]
        public object Get()
        {
            return new
            {
                RequestId = DateTime.Now.Ticks,
                ServiceName = _configuration.GetValue<string>("ConsulService:ServiceName"),
                RequestUrl = UriHelper.GetDisplayUrl(Request),
                RequestAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                LocalAddress = Request.HttpContext.Connection.LocalIpAddress.ToString(),
                CreationTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
