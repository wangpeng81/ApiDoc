using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ApiDoc.DAL
{
    public class UtilConf
    {
        private static IConfiguration config;
        public static IConfiguration Configuration//加载配置文件
        {
            get
            {
                if (config != null) return config;
                config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                return config;
            }
            set => config = value;
        }

        public static string GetConnectionString(string key)
        {
            string connection = Configuration.GetConnectionString(key);
            return connection;
        }
    }
}
