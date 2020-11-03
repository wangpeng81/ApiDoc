using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApiDoc.Controllers
{
    /// <summary>
    /// 步骤相当操作
    /// </summary>
    public class StepController : Controller
    {
        private readonly IInterfaceDAL interfaceDAL;
        private readonly IFlowStepDAL flowStepDAL;
        private readonly MyConfig myConfig;
        private readonly IConfiguration configuration;

        public StepController(IInterfaceDAL interfaceDAL, 
            IFlowStepDAL flowStepDAL, 
            MyConfig myConfig,
            IConfiguration configuration )
        {
            this.interfaceDAL = interfaceDAL;
            this.flowStepDAL = flowStepDAL;
            this.myConfig = myConfig;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        //返回步骤页面 
        private IActionResult Redist(int FKSN)
        {
            if( FKSN > 0 )
            {
                string DataBaseType = this.interfaceDAL.Get<InterfaceModel>(FKSN).DataType;
                List<string> DataBases = this.myConfig[DataBaseType].DataBases;
                ViewData["DataBase"] = DataBases;
            }

            //数据库
            List<string> CommandType = configuration.GetSection("CommandType").Get<List<string>>();
            ViewData["CommandType"] = CommandType;

            List <FlowStepModel> list = this.flowStepDAL.Query(FKSN);
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
                FlowStepModel modelOld = this.flowStepDAL.Get<FlowStepModel>(model.SN);
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
            int resutl = this.flowStepDAL.DeleteAll(model.SN);
            return this.Redist(model.FKSN);
        }

        [HttpPost]
        public int SaveCmdText(int SN, string CommandType, string CommandText, string DataBase)
        {
            return this.flowStepDAL.SaveCmdText(SN, CommandType, CommandText, DataBase);
        }


    }
}
