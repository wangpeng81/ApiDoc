using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks; 
using ApiDoc.DAL;
using ApiDoc.IBLL;
using ApiDoc.IDAL;
using ApiDoc.Middleware;
using ApiDoc.Models;
using ApiDoc.Utility.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApiDoc.Controllers
{
    [CustomExceptionFilterAttribute]
    public class InterfaceController : Controller
    {
        private readonly IInterfaceDAL infterfaceDAL;
        private readonly IFlowStepDAL flowStepDAL;
        private readonly IFlowStepHisDAL flowStepHisDAL;
        private readonly DBRouteValueDictionary routeDict;
        private readonly IParamDAL paramDAL;
        private readonly IInterfaceBLL interfaceBLL;
        private readonly IConfiguration config;

        public InterfaceController(IInterfaceDAL infterfaceDAL, 
            IFlowStepDAL flowStepDAL,
            IFlowStepHisDAL flowStepHisDAL,
            DBRouteValueDictionary _routeDict,
            IParamDAL paramDAL,
            IInterfaceBLL interfaceBLL,
            IConfiguration config)
        {
            this.infterfaceDAL = infterfaceDAL;
            this.flowStepDAL = flowStepDAL;
            this.flowStepHisDAL = flowStepHisDAL;
            this.routeDict = _routeDict;
            this.paramDAL = paramDAL;
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

        #region 接口

        public IActionResult Add(int FKSN, int SN)
        {
            List<string> list = new List<string>();
            list.Add("DataSet");
            list.Add("Int");
            list.Add("Scalar");
            ViewData.Add("ExecuteType", list); //执行集合类型

            //参数类型
            List<string> DataType = new List<string>();
            DataType.Add("varchar");
            DataType.Add("int");
            DataType.Add("datetime");
            DataType.Add("float");
            ViewData.Add("DataType", DataType);

            List<string> stList = new List<string>();
            stList.Add("");
            stList.Add("Json");
            stList.Add("Xml");
            ViewData.Add("SerializeType", stList);

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
                DBInterfaceModel dbInter = this.interfaceBLL.GetInterfaceModel(SN);
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
                DBInterfaceModel dbInter = this.interfaceBLL.GetInterfaceModel(SN);
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
         
        #endregion
          
        #region Step

        private IActionResult Redist(int FKSN)
        {
            string[] dataBase = this.config.GetSection("DataBase").Get<string[]>();
            ViewData["DataBase"] = dataBase;
            List<FlowStepModel> list = this.flowStepDAL.Query(FKSN);
            return PartialView("/Views/Interface/FlowStepList.cshtml", list);
        }
        public IActionResult FlowStepList(int FKSN)
        {  
            return this.Redist(FKSN); 
        }

        [HttpPost]
        public IActionResult StepSave(FlowStepModel model)
        {
            if (model.SN > 0)
            {
                FlowStepModel modelOld = this.flowStepDAL.Get< FlowStepModel>(model.SN);  
                model.CommandText = modelOld.CommandText;
                model.CommandType = modelOld.CommandType;
                model.DataBase = modelOld.DataBase;
                this.flowStepDAL.Update(model);
            }
            else
            {
                int SN = this.flowStepDAL.Insert(model);
                model.SN = SN;
            }
            return Redist(model.FKSN);
             
        }

        public IActionResult DeleteFlowStep(FlowStepModel model)
        {
            int resutl = this.flowStepDAL.Delete(model); 
            return this.Redist(model.FKSN);
        }

        [HttpPost]
        public int SaveCmdText(int SN, string CommandType, string CommandText, string DataBase)
        {
           return this.flowStepDAL.SaveCmdText(SN, CommandType, CommandText, DataBase);
        }

        #endregion

        #region 历史His

        [HttpPost]
        public IActionResult StepHisList(int FKSN)
        {
            ViewData.Add("FKSN", FKSN);

            List<FlowStepHisModel> list = this.flowStepHisDAL.Query(FKSN);
            return PartialView("/Views/Interface/FlowStepHisList.cshtml", list);
        }

        [HttpPost]
        public IActionResult StepHisAdd(FlowStepHisModel model)
        {
            model.DTime = System.DateTime.Now;
            this.flowStepHisDAL.Insert(model);

            return this.StepHisList(model.FKSN); 
        }

        [HttpPost]
        public IActionResult StepHisDelete(List<int> ids, int FKSN)
        {
            foreach (int SN in ids)
            {
                FlowStepHisModel model = new FlowStepHisModel();
                model.SN = SN;
                int i = this.flowStepHisDAL.Delete(model); 
            }

            return this.StepHisList(FKSN);
        }

        [HttpPost]
        public IActionResult SmoExecute(int FKSN, List<int> ids)
        {
            foreach (int id in ids)
            {
                FlowStepModel flowModel = this.flowStepDAL.Get<FlowStepModel>(FKSN);
                 
                foreach (FlowStepHisModel hisModel in this.flowStepHisDAL.Query(FKSN))
                {
                    if (hisModel.SN == id)
                    {
                        hisModel.IsEnable = true;
                        string script = hisModel.Text;
                        if( script != "" )
                        {
                            string procName = hisModel.FileName.Split(".")[0].Split("-")[0]; 
                            this.flowStepHisDAL.SmoExecute(flowModel.DataBase, procName, script);
                        }
                    }
                    else
                    {
                        hisModel.IsEnable = false;
                    }
                    this.flowStepHisDAL.Update(hisModel);
                }

            }
            return this.StepHisList(FKSN);
        }


        #endregion

        #region 参数

        [HttpPost]
        public IActionResult ParamList(int FKSN)
        { 
            List<ParamModel> list = this.paramDAL.Query(FKSN);
            return PartialView("/Views/Interface/ParamList.cshtml", list);
        }

        [HttpPost]
        public IActionResult ParamAdd(ParamModel model)
        {
            if (model.SN > 0)
            {
                this.paramDAL.Update(model);
            }
            else
            { 
                this.paramDAL.Insert(model);
            }
            return ParamList(model.FKSN);
        }

        [HttpPost]
        public IActionResult ParamDelete(List<int> ids, int FKSN)
        {
            foreach (int SN in ids)
            {
                ParamModel model = new ParamModel();
                model.SN = SN;
                int i = this.paramDAL.Delete(model);
            }

            return ParamList(FKSN);
        }
        #endregion

    }
}

