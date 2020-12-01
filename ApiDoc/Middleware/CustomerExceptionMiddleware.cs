using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiDoc.Middleware
{
    /// <summary>
    /// 自定义异常中间件
    /// </summary>
    public class CustomerExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomerExceptionMiddleware> _logger;
        public CustomerExceptionMiddleware(RequestDelegate next, ILogger<CustomerExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await ExceptionHandlerAsync(context, e);
            }
        }

        private async Task ExceptionHandlerAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status200OK;
            _logger.LogError($"系统出现错误：{ex.Message}--{ex.StackTrace}"); 
            var result = new
            {
                status = 500,
                msg = ex.Message
            };
             
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }

    /// <summary>
    /// 静态类
    /// </summary>
    public static class ExceptionMiddlewareExtension
    {
        /// <summary>
        /// 静态方法
        /// </summary>
        /// <param name="app">要进行扩展的类型</param>
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware(typeof(CustomerExceptionMiddleware));
        }
    }
}
