using ApiDoc.Models.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiDoc.Middleware.Author
{
    public class JWTHSAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DBRouteValueDictionary routeDict; 
        private JWTTokenOptions jWTTokenOptions;

        public JWTHSAuthorizeMiddleware(RequestDelegate next,
            DBRouteValueDictionary routeDict, 
            MyConfig myConfig )
        {
            this._next = next;
            this.routeDict = routeDict; 
            jWTTokenOptions = myConfig.JWTTokenOptions; 
        }

        public async Task Invoke(HttpContext context)
        {
            //JsonWebTokenValidate 
            //若是路径可以匿名访问，直接跳过 
            string path = context.Request.Path.ToString();
            if (this.routeDict.ContainsKey(path) || path == "/Author/Index")
            {
                var result = context.Request.Headers.TryGetValue("Authorization", out StringValues authStr);
                if (!result || string.IsNullOrEmpty(authStr.ToString()))
                {
                    await this.WriteAsync(context, "没有授权无法访问!");
                    return;
                }

                //进行验证与自定义验证 
                //string encodeJwt = authStr.ToString().Substring("Bearer ".Length).Trim();
                string encodeJwt = authStr.ToString().Trim();

                string msg = Validate(encodeJwt , jWTTokenOptions);
                if (msg != "")
                { 
                    await this.WriteAsync(context, msg);
                    return;
                }

                await _next(context);
            }
            else
            {
                await _next(context);
            }; 
           
        }

        private async Task WriteAsync(HttpContext context, string text)
        {
            context.Response.ContentType = "text/plain;charset=utf-8";
            await context.Response.WriteAsync(text);
        }

        public string Validate(string encodeJwt, JWTTokenOptions setting)
        {
            string msg = "";
            var success = true;
            var jwtArr = encodeJwt.Split('.');
            var header = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[0]));
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[1]));

            //首先验证签名是否正确（必须的） 
            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(setting.SecurityKey));
            byte[] buffer = Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1]));
            string encode = Base64UrlEncoder.Encode(hs256.ComputeHash(buffer));
            string sign = jwtArr[2];

            success = string.Equals(sign, encode);
            if (!success)
            {
                msg = "签名不正确";
                return msg;//签名不正确直接返回 
            }

            //其次验证是否在有效期内（也应该必须） 
            var now = ToUnixEpochDate(DateTime.UtcNow);
            success = (now < long.Parse(payLoad["exp"].ToString()));
            if (!success)
            {
                msg = "授权码不在有效期内";
                return msg;//签名不正确直接返回 
            }

            return msg;
        }
 
        private long ToUnixEpochDate(DateTime date)
        {
            long time = (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
            return time;
        }
  
    }
}
