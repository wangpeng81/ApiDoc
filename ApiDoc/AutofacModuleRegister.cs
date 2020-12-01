

using ApiDoc.AuthenticationCenter;
using ApiDoc.Middleware;
using ApiDoc.Models.Components;
using ApiDoc.Utility;
using Autofac;
using JMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;


namespace ApiDoc
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //builder.RegisterAssemblyTypes(assemblies).Where(b => b.GetInterfaces().
            // Any(c => c == baseType && b != baseType)).AsImplementedInterfaces().InstancePerLifetimeScope();
            ////自动注册控制器
            //builder.RegisterControllers(assemblies);
 
            string path = Assembly.GetExecutingAssembly().Location; 
            string[] dllFiles = new string[] { "ApiDoc.DAL.dll", "ApiDoc.BLL.dll" };
            var basePath = path.Replace("ApiDoc.dll", "");
            List<Assembly> assList = new List<Assembly>();
            foreach (string file in dllFiles)
            {
                var businessDALFile = Path.Combine(basePath, file);
                var assemblysBusiness = Assembly.LoadFrom(businessDALFile);
                assList.Add(assemblysBusiness);
            }
            builder.RegisterAssemblyTypes(assList.ToArray())
               .AsImplementedInterfaces() //对每一个依赖或每一次调用创建一个新的唯一的实例。这也是默认的创建实例的方式。
               .InstancePerDependency();
 
            //注册路由集合
            builder.RegisterType<DBRouteValueDictionary>().SingleInstance();
            //builder.RegisterType<MyConfig>().SingleInstance(); //配置文件，验证码，数据库的连接方式
 
            string jsonName = basePath + "MyConfig";
            MyConfig  myConfig = new JsonFileHelper(jsonName).Read<MyConfig>();
             
            builder.RegisterInstance(myConfig).SingleInstance();

            //注册数据库
            builder.RegisterType<OracleConnection>().Named<IDbConnection>("Oracle");
            builder.RegisterType<OracleDataAdapter>().Named<IDbDataAdapter>("Oracle");
            builder.RegisterType<OracleParameter>().Named<DbParameter>("Oracle");

            builder.RegisterType<SqlConnection>().Named<IDbConnection>("SqlServer");
            builder.RegisterType<SqlDataAdapter>().Named<IDbDataAdapter>("SqlServer");
            builder.RegisterType<SqlParameter>().Named<DbParameter>("SqlServer");

            //MySql.Data.MySqlClient
            builder.RegisterType<MySqlConnection>().Named<IDbConnection>("MySql");
            builder.RegisterType<MySqlDataAdapter>().Named<IDbDataAdapter>("MySql");
            builder.RegisterType<MySqlParameter>().Named<DbParameter>("MySql");
  
            #region RS256 
            builder.RegisterType<JWTHSService>().
                As<IJWTService>().
                InstancePerLifetimeScope(); //对等验证

            //builder.RegisterType<JWTRSService>().
            //    As<IJWTService>().
            //    InstancePerLifetimeScope(); //非对等验证
            #endregion
             
        }
    }

}
