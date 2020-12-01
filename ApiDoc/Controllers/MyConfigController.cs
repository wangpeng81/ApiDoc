using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ApiDoc.Models.Components;
using ApiDoc.Utility;
using JMS.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiDoc.Controllers
{
    /// <summary>
    /// 配置页相关操作
    /// </summary>
    public class MyConfigController : BaseController
    {
        private readonly MyConfig myConfig;
        private readonly ILogger<MyConfigController> logger;
        private JsonFileHelper jsonFileHelper;

        public MyConfigController(IHostEnvironment hostingEnvironment, 
            MyConfig myConfig, ILogger<MyConfigController> logger )
        {
            string path = hostingEnvironment.ContentRootPath + "\\MyConfig"; 
            jsonFileHelper = new JsonFileHelper(path);
            this.myConfig = myConfig;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            ViewData["Nav"] = base.LoadNav("MyConfig"); 
            MyConfig myConfig1 = jsonFileHelper.Read<MyConfig>(); 
            return View(myConfig1);
        }

        public int SaveAuthorize(string SecurityKey)
        {
            JWTTokenOptions jWTTokenOptions = new JWTTokenOptions();
            jWTTokenOptions.Audience = myConfig.JWTTokenOptions.Audience;
            jWTTokenOptions.Issuer = myConfig.JWTTokenOptions.Issuer;
            jWTTokenOptions.SecurityKey = SecurityKey;
            jsonFileHelper.Write<JWTTokenOptions>("JWTTokenOptions", jWTTokenOptions);
            myConfig.JWTTokenOptions = jWTTokenOptions;
            return 100;
        }

        [HttpPost]
        public int SaveDataType(DataBases DataType)
        { 
            jsonFileHelper.Write<DataBases>("DataType", DataType);
            myConfig.DataType = DataType;
            return 100;
        }

        [HttpPost]
        public int SaveGateWay(NetAddress netAddress)
        { 
            jsonFileHelper.Write<NetAddress>("GatewayAddress", netAddress);
            myConfig.GatewayAddress = netAddress;
            return 100;
        }

        [HttpGet]
        //获取JMS服务列表
        public string ListMicroService()
        {
            string services = "";

            string Address = this.myConfig.GatewayAddress.Address;
            int port = this.myConfig.GatewayAddress.Port;

            try
            {
                
                JMS.JMSClient tran = new JMS.JMSClient(Address, port);
                RegisterServiceRunningInfo[] registerServiceRunningInfos = tran.ListMicroService("");
                if (registerServiceRunningInfos.Length > 0)
                {
                    foreach (string serverName in registerServiceRunningInfos[0].ServiceNames)
                    {
                        if (services != "")
                        {
                            services += "\n";
                        }
                        services += serverName;
                    }
                }

                logger.LogError($"Address:{Address} Port{port}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Address:{Address} Port{port}" + ex.Message);
            }

            return services;
        }

        [HttpGet]
        //生成公钥
        public string CreatePublickKey()
        { 
            RSAParameters publicKeys;
            using (var rsa = new RSACryptoServiceProvider(1024))//即时生成
            {
                try
                { 
                    publicKeys = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
              
            string key = Convert.ToBase64String(publicKeys.Modulus);
           // int length = key.Length;
            //string str = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDI2a2EJ7m872v0afyoSDJT2o1 SitIeJSWtLJU8/Wz2m7gStexajkeD Lka6DSTy8gt9UwfgVQo6uKjVLG5Ex7PiGOODVqAEghBuS7JzIYU5RvI543nNDAPfnJsas96mSA7L/mD7RTE2drj6hf3oZjJpMPZUQI/B1Qjb5H3K3PNwIDAQAB";
            //int length1 = str.Length;
            return key;
        } 
    }
}
