using ApiDoc.Middleware;
using ApiDoc.Middleware.Author;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc
{
    public static class ConfigureExtensions
    {
        public static void ConfigureApdDoc(this IApplicationBuilder app)
        {
            app.UseMiddleware<MyCorsMiddleware>();
            //app.UseMiddleware<JWTRSAuthorizeMiddleware>(); //验证
            app.UseMiddleware<JWTHSAuthorizeMiddleware>();
            //app.UseMiddleware<JWTMiddleware>();
            //app.UseMiddleware<AuthorizeMiddleware>();//JWTMiddleware
            app.UseMiddleware<DBMiddleware>();

            app.UseAuthentication(); //鉴权：解析信息--就是读取token，解密token 
        }
    }
}

