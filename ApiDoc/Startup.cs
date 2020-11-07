using System; 
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ApiDoc.AuthenticationCenter;
using ApiDoc.Middleware;
using ApiDoc.Middleware.Author;
using ApiDoc.Models.Components;
using ApiDoc.Utility;
using ApiDoc.Utility.Filter;
using Autofac;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ApiDoc
{
    public class Startup
    { 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置跨域处理
            services.AddCors(options => options.AddPolicy("CorsPolicy",
               builder =>
               {
                   builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin(); //允许任何来源的主机访问 ;
               }));

            #region jwt校验 

            //增加读取配置信息  

            string path = Assembly.GetExecutingAssembly().Location;
            var basePath = path.Replace("ApiDoc.dll", "");
            string jsonName = basePath + "MyConfig";
            MyConfig myConfig = new JsonFileHelper(jsonName).Read<MyConfig>();

            //RS
            //在中间件管道中启用authentication中间件
            //string path1 = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            //string key = File.ReadAllText(path1);
            //var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            //SecurityKey issuerSigningKey = new RsaSecurityKey(keyParams);

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

            #endregion

            //services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
           

            services.AddControllersWithViews(options => {
                //options.Filters.Add(typeof(CustomExceptionFilterAttribute));
                //options.Filters.Add<JWTAuthorizeFilter>();
            });
 

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //DAL 

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //添加依赖注入实例，AutofacModuleRegister就继承自Autofac.Module的类
            builder.RegisterModule(new AutofacModuleRegister());
 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {  
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });

            app.UseMiddleware<MyCorsMiddleware>();
            //app.UseMiddleware<JWTRSAuthorizeMiddleware>(); //验证
            app.UseMiddleware<JWTHSAuthorizeMiddleware>();
            //app.UseMiddleware<JWTMiddleware>();
            //app.UseMiddleware<AuthorizeMiddleware>();//JWTMiddleware
            app.UseMiddleware<DBMiddleware>();

            app.UseAuthentication(); //鉴权：解析信息--就是读取token，解密token 
            app.UseRouting(); 
            app.UseAuthorization();

            app.UseCors("CorsPolicy");  //跨域 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
 
        }
    }
}
