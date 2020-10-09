using ApiDoc.Middleware;
using ApiDoc.Models;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ApiDoc
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var businessDllFile = Path.Combine(basePath, "ApiDoc.DAL.dll");
            var assemblysBusiness = Assembly.LoadFrom(businessDllFile);
            builder.RegisterAssemblyTypes(assemblysBusiness)
            .AsImplementedInterfaces()
            .InstancePerDependency();

            //注册路由集合
            builder.RegisterType<DBRouteValueDictionary>().AsSelf().SingleInstance();

            //注册数据库
            builder.RegisterType<SqlConnection>().As<IDbConnection>();
            builder.RegisterType<SqlCommand>().As<IDbCommand>();
            builder.RegisterType<SqlDataAdapter>().As<IDataAdapter>();

        }
    }
}
