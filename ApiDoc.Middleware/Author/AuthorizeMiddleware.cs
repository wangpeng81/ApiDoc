using ApiDoc.Models.Components;
using ApiDoc.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Middleware
{
    public class AuthorizeMiddleware
    {
        const string Authorize = "Authorize";

        private readonly RequestDelegate next;
        private readonly DBRouteValueDictionary routeDict;
        private readonly MyConfig myConfig;

        public AuthorizeMiddleware(RequestDelegate next, 
            DBRouteValueDictionary _routeDict, 
            MyConfig myConfig,
            IHostEnvironment hostingEnvironment
            )
        {
            this.next = next;
            routeDict = _routeDict;

            string path = hostingEnvironment.ContentRootPath + "\\MyConfig"; 
            //myConfig = new JsonFileHelper(path).Read<MyConfig>();
            this.myConfig = myConfig; 
        }

        public async Task Invoke(HttpContext context )
        { 
            string path = context.Request.Path.ToString();
            if (this.routeDict.ContainsKey(path))
            {
                if (this.myConfig.JWTTokenOptions.SecurityKey != "")
                {
                    if (!context.Request.Headers.ContainsKey("Authorize"))
                    {
                        await this.WriteAsync(context, "没有授权无法访问!");
                    }
                    else
                    {
                        string authorize = context.Request.Headers["Authorize"];
                        if (authorize == this.myConfig.JWTTokenOptions.SecurityKey)
                        {
                            await next(context);
                        }
                        else
                        {
                            await this.WriteAsync(context, "授权码错误无法访问!");
                        }
                    }
                }
                else
                {
                    await next(context);
                }
            }
            else
            {
                await next(context);
            } 
        }

        private async Task WriteAsync(HttpContext context, string text)
        { 
            context.Response.ContentType = "text/plain;charset=utf-8";
            await context.Response.WriteAsync(text);
        }
    }
}
