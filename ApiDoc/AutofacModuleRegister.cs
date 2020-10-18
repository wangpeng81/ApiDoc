using ApiDoc.Middleware;
using ApiDoc.Models;
using Autofac;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System.Data;
using System.IO; 
using System.Reflection; 

namespace ApiDoc
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            string[] dllFiles = new string[] { "ApiDoc.DAL.dll", "ApiDoc.BLL.dll" };
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            foreach(string file in dllFiles)
            {
                var businessDALFile = Path.Combine(basePath, file);
                var assemblysBusiness = Assembly.LoadFrom(businessDALFile);
                builder.RegisterAssemblyTypes(assemblysBusiness)
                .AsImplementedInterfaces()
                .InstancePerDependency();
            } 

            //注册路由集合
            builder.RegisterType<DBRouteValueDictionary>().SingleInstance();
 
            //注册数据库
            builder.RegisterType<SqlConnection>().As<IDbConnection>();
            builder.RegisterType<SqlDataAdapter>().As<IDbDataAdapter>();
        }
    }
}
