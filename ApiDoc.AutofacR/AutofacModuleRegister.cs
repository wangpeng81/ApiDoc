﻿

using ApiDoc.Middleware;
using Autofac;
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


namespace ApiDoc.AutofacR
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            string path = Assembly.GetExecutingAssembly().Location;

            string[] dllFiles = new string[] { "ApiDoc.DAL.dll", "ApiDoc.BLL.dll" };
            var basePath = path.Replace("ApiDoc.AutofacR.dll", "");
            List<Assembly> assList = new List<Assembly>();
            foreach (string file in dllFiles)
            {
                var businessDALFile = Path.Combine(basePath, file);
                var assemblysBusiness = Assembly.LoadFrom(businessDALFile);
                assList.Add(assemblysBusiness);
            }
            builder.RegisterAssemblyTypes(assList.ToArray())
               .AsImplementedInterfaces()
               .InstancePerDependency();

            //var dataAccess = Assembly.GetExecutingAssembly();
            //builder.RegisterAssemblyTypes(dataAccess)
            //    .AsImplementedInterfaces();//对每一个依赖或每一次调用创建一个新的唯一的实例。这也是默认的创建实例的方式。

            //注册路由集合
            builder.RegisterType<DBRouteValueDictionary>().SingleInstance();

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

            //builder.RegisterType<Logger<BaseDAL>>();
            //builder.RegisterType<BaseDAL>().PropertiesAutowired();

        }
    }

}