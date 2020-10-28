using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ApiDoc.Utility
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class Log4netHelper
    {
        private static Log4NetProvider instance;

        
        //public static ILoggerRepository Repository { get; set; }
        /// <summary>
        /// 输出Erro日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            StackTrace trace = new StackTrace();
            var className = trace.GetFrame(1).GetMethod().DeclaringType;
            WriteLog(LogLevel.Error, message, className.FullName); 
        }

        /// <summary>
        /// 输出Warning日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warning(string message)
        {
            StackTrace trace = new StackTrace();
            var className = trace.GetFrame(1).GetMethod().DeclaringType;
            WriteLog(LogLevel.Warning, message, className.FullName);
        }

        /// <summary>
        /// 输出Info日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            StackTrace trace = new StackTrace();  
            var className = trace.GetFrame(1).GetMethod().DeclaringType;  
            //MethodBase method = trace.GetFrame(1).GetMethod(); 

            //记录日志
            WriteLog(LogLevel.Information, message, className.FullName);
        }
        /// <summary>
        /// 输出Debug日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Debug(string message)
        {
            StackTrace trace = new StackTrace(); 
            var className = trace.GetFrame(1).GetMethod().DeclaringType; 
            WriteLog(LogLevel.Debug, message, className.FullName); 
        }
         
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="message">日志信息</param>
        /// <param name="type">类名 方法名</param>
        private static void WriteLog(LogLevel logLevel, string message, string categoryName)
        {
            if (instance == null)
            {
                instance = new Log4NetProvider();
            }

            ILogger Log = getInstance().CreateLogger(categoryName);

            switch (logLevel)
            {
                case LogLevel.Debug:
                    Log.LogDebug(message);
                    break;
                case LogLevel.Error:
                    Log.LogError(message);
                    break;
                case LogLevel.Information:
                    Log.LogInformation(message);
                    break;
                case LogLevel.Warning:
                    Log.LogWarning(message);
                    break;
            } 
        }
 
        public static ILogger CreateLogger<T>() where T : class
        {
            ILogger Log = getInstance().CreateLogger<T>();
            return Log;
        }

        private static Log4NetProvider getInstance()
        {
            if (instance == null)
            {
                instance = new Log4NetProvider();
            }
            return instance;
        }
    }

}
