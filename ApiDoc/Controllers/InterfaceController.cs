using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks; 
using ApiDoc.DAL; 
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
        private readonly IConfiguration config;

        public InterfaceController(IInterfaceDAL infterfaceDAL, 
            IFlowStepDAL flowStepDAL,
            IFlowStepHisDAL flowStepHisDAL,
            DBRouteValueDictionary _routeDict,
            IConfiguration config)
        {
            this.infterfaceDAL = infterfaceDAL;
            this.flowStepDAL = flowStepDAL;
            this.flowStepHisDAL = flowStepHisDAL;
            this.routeDict = _routeDict;
            this.config = config;
        } 

        public IActionResult Index(string title,string url, int fksn)
        { 
            ViewData.Add("FKSN", fksn);
            ViewData.Add("keyTitle", title);
            ViewData.Add("keyUrl", url);

            List<InterfaceModel> list = this.infterfaceDAL.All(title,url, fksn);
            return View(list);
        }

        public IActionResult Add(int FKSN, int SN)
        {
            List<string> list = new List<string>();
            list.Add("DataSet");
            list.Add("Int");
            list.Add("Scalar"); 
            ViewData.Add("ExecuteType", list); //执行集合类型
           

            InterfaceModel model = new InterfaceModel();
            model.SN = SN;
            model.FKSN = FKSN;
            if (SN > 0)
            {
                model =(InterfaceModel)this.infterfaceDAL.Get(model);　
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
                Models.InterfaceModel model = new InterfaceModel();
                model.SN = sn;
                int result = this.infterfaceDAL.Delete(model);
            }
            return 1;
        }

        [HttpPost]
        public InterfaceModel Save(InterfaceModel model)
        { 
            if (model.SN > 0)
            {  
                this.infterfaceDAL.Update(model); 
            }
            else
            {
                int SN = this.infterfaceDAL.Insert(model);
                model.SN = SN; 
            } 
            return model;
        }

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
                FlowStepModel modelOld = new FlowStepModel();
                modelOld.SN = model.SN; 
                modelOld = (FlowStepModel)this.flowStepDAL.Get(modelOld);
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

            List<FlowStepHisModel> list = this.flowStepHisDAL.Query(model.FKSN);
            return PartialView("/Views/Interface/FlowStepHisList.cshtml", list);
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
          
            List<FlowStepHisModel> list = this.flowStepHisDAL.Query(FKSN);
            return PartialView("/Views/Interface/FlowStepHisList.cshtml", list);
        }

        [HttpPost]
        public int RunProcSql(List<int> ids)
        {

            return 0;
        }

        #endregion

    }
}

