using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;  
using ApiDoc.IBLL;
using ApiDoc.IDAL;
using ApiDoc.Middleware;
using ApiDoc.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiDoc.Controllers
{ 
    public class InterfaceController : Controller
    {
        private readonly IInterfaceDAL infterfaceDAL; 
        private readonly DBRouteValueDictionary routeDict;
        
        private readonly IInterfaceBLL interfaceBLL;
        private readonly IConfiguration config;

        public InterfaceController(IInterfaceDAL infterfaceDAL,  
            DBRouteValueDictionary _routeDict, 
            IInterfaceBLL interfaceBLL,
            IConfiguration config)
        {
            this.infterfaceDAL = infterfaceDAL; 
            this.routeDict = _routeDict; 
            this.interfaceBLL = interfaceBLL;
            this.config = config;
        } 

        public IActionResult Index(string title,string url, int fksn)
        { 
            ViewData.Add("FKSN", fksn);
            ViewData.Add("keyTitle", title);
            ViewData.Add("keyUrl", url);
            ViewData.Add("host", Url.Content("~") );
            ;

            List<InterfaceModel> list = this.infterfaceDAL.All(title,url, fksn);
            return View(list);
        }
 
        public IActionResult Add(int FKSN, int SN)
        {
            List<string> list = this.config.GetSection("ExecuteType").Get<List<string>>(); 
            ViewData.Add("ExecuteType", list); //执行集合类型

            //参数类型
            List<string> DataType = this.config.GetSection("DataType").Get<List<string>>(); 
            ViewData.Add("DataType", DataType);

            //序列化格式
            List<string> stList = this.config.GetSection("SerializeType").Get<List<string>>(); 
            ViewData.Add("SerializeType", stList);

            //数据库类型
            List<string> dataBaseType = this.config.GetSection("DataBaseType").Get<List<string>>(); 
            ViewData.Add("DataBaseType", dataBaseType);

            InterfaceModel model = new InterfaceModel();
            model.SN = SN;
            model.FKSN = FKSN;
            if (SN > 0)
            {
                model = this.infterfaceDAL.Get<InterfaceModel>(SN);
            }

            ViewData.Add("FullPath", this.infterfaceDAL.FullPath(FKSN)); //虚拟路径全称
            return View(model);
        }

        public List<InterfaceModel> All(int fksn)
        {
            List<InterfaceModel> list = this.infterfaceDAL.All("", "", fksn);
            return list;
        }

        [HttpPost]
        public int Delete(List<int> ids)
        {
            foreach (int sn in ids)
            {
                Models.InterfaceModel model = this.infterfaceDAL.Get<InterfaceModel>(sn);
                model.SN = sn; 
                int result = this.infterfaceDAL.Delete(model);

                if (this.routeDict.ContainsKey(model.Url))
                {
                    this.routeDict.Remove(model.Url);
                }
            }
            return 1;
        }

        [HttpPost]
        public InterfaceModel Save(InterfaceModel model)
        {
            int SN = model.SN;
            if (model.SN > 0)
            {
                this.infterfaceDAL.Update(model);
            }
            else
            {
                SN = this.infterfaceDAL.Insert(model);
                model.SN = SN;
            }

            //更新路由
            if (!model.IsStop)
            {
                InterfaceModel dbInter = this.interfaceBLL.GetInterfaceModel(SN);
                string url = model.Url;
                if (this.routeDict.ContainsKey(url))
                {
                    this.routeDict[url] = dbInter;
                }
                else
                {
                    this.routeDict.Add(url, dbInter);
                }
            }

            return model;
        }

        [HttpGet]
        public int UpdateStop(int SN, bool bStop)
        {
            InterfaceModel model = this.infterfaceDAL.Get<InterfaceModel>(SN);
            model.IsStop = bStop;
            if (model.SN > 0)
            {
                this.infterfaceDAL.Update(model);
            }
            else
            {
                SN = this.infterfaceDAL.Insert(model);
                model.SN = SN;
            }

            //更新路由
            string url = model.Url;
            if (model.IsStop)
            {
                if (this.routeDict.ContainsKey(url))
                {
                    this.routeDict.Remove(url);
                }
            }
            else //启用
            {
                InterfaceModel dbInter = this.interfaceBLL.GetInterfaceModel(SN);
                if (this.routeDict.ContainsKey(url))
                {
                    this.routeDict[url] = dbInter;
                }
                else
                {
                    this.routeDict.Add(url, dbInter);
                }
            }

            return model.SN;
        }
         
    }
}

