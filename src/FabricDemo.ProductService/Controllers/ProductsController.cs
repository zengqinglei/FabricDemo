using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace FabricDemo.ProductService.Controllers
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public class ProductsController : BaseController
    {

        private readonly IConfiguration _configuration;

        /// <inheritdoc />
        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 产品服务输出示例
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
