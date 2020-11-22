

using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ApiDoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            IHostBuilder hostBilder = CreateHostBuilder(args);
            IHost host = hostBilder.Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
           
            IHostBuilder hostBilder = Host.CreateDefaultBuilder(args); 
            hostBilder.ConfigureLogging((context, loggingBuilder)=> {

                loggingBuilder.ClearProviders();
                loggingBuilder.AddLog4Net(new Log4NetProviderOptions()
                {
                    Log4NetConfigFileName = "log4net.config",
                    Watch = true
                });
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
            return hostBilder;

            //return Host.CreateDefaultBuilder(args)
                //.ConfigureLogging((context, loggingBuilder)=> {
                //    loggingBuilder.AddFilter("System", LogLevel.Warning); //不写　System　类型的日志
                //    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                //    loggingBuilder.AddLog4Net();//使用log4Net
                //})
                
        }
            
    }
}
