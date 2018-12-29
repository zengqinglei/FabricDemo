using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FabricDemo.ApiGateway.Middlewares
{
    /// <summary>
    ///     请求日志中间件
    /// </summary>
    public class RequestLogMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <inheritdoc />
        public RequestLogMiddleware(RequestDelegate next, ILogger<RequestLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 读取请求内容
        /// </summary>
        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            return bodyAsText;
        }

        /// <summary>
        /// 读取输出内容
        /// </summary>
        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        /// <summary>
        ///     调用方法
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            _logger.LogInformation(
                "[request] {ipAddress} {duration} {method} {statusCode} {requestUrl}",
                context.Connection.RemoteIpAddress.ToString(),
                $"{stopwatch.ElapsedMilliseconds.ToString()} ms",
                context.Request.Method,
                context.Response.StatusCode.ToString(),
                UriHelper.GetDisplayUrl(context.Request));
        }
    }

    /// <summary>
    ///     请求处理中间件拓展
    /// </summary>
    public static class RequestLogMiddlewareExtensions
    {
        /// <summary>
        /// before calling .UseStaticFiles method.
        /// </summary>
        public static IApplicationBuilder UseRequestLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
