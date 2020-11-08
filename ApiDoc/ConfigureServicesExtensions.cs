using ApiDoc.Models.Components;
using ApiDoc.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiDoc
{
    public static class ConfigureServicesExtensions
    {
        public static void ConfigureServicesApiDoc(this IServiceCollection services)
        {

            //增加读取配置信息  

            //RS
            //在中间件管道中启用authentication中间件
            //string path1 = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            //string key = File.ReadAllText(path1);
            //var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            //SecurityKey issuerSigningKey = new RsaSecurityKey(keyParams);


            string path = Assembly.GetExecutingAssembly().Location;
            var basePath = path.Replace("ApiDoc.dll", "");
            string jsonName = basePath + "MyConfig";
            MyConfig myConfig = new JsonFileHelper(jsonName).Read<MyConfig>();

            //HS----
            SecurityKey issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(myConfig.JWTTokenOptions.SecurityKey));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,  //是否验证SecurityKey
                    ValidateIssuer = false,          //是否验证Issuer 
                    ValidateAudience = false,       //是否验证Audience
                    ValidateLifetime = false,        //是否验证失效时间
                    IssuerSigningKey = issuerSigningKey,
                    ValidIssuer = myConfig.JWTTokenOptions.Issuer,
                    ValidAudience = myConfig.JWTTokenOptions.Audience  //是否验证Audience 
                };
            });
        }
    }
}
