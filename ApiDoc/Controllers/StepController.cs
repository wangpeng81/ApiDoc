using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApiDoc.Controllers
{
    public class StepController : Controller
    {
        private readonly IFlowStepDAL flowStepDAL;
        private readonly IConfiguration config;

        public StepController(IFlowStepDAL flowStepDAL, IConfiguration config)
        {
            this.flowStepDAL = flowStepDAL;
            this.config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

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
            int resutl = this.flowStepDAL.Delete(model);
            return this.Redist(model.FKSN);
        }

        [HttpPost]
        public int SaveCmdText(int SN, string CommandType, string CommandText, string DataBase)
        {
            return this.flowStepDAL.SaveCmdText(SN, CommandType, CommandText, DataBase);
        }

    }
}
