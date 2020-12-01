using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiDoc.Controllers
{
    /// <summary>
    /// 历史脚本
    /// </summary>
    public class HisController : BaseController
    {
        private readonly IFlowStepDAL flowStepDAL;
        private readonly IFlowStepHisDAL flowStepHisDAL;

        public HisController(IFlowStepDAL flowStepDAL, IFlowStepHisDAL flowStepHisDAL)
        {
            this.flowStepDAL = flowStepDAL;
            this.flowStepHisDAL = flowStepHisDAL;
        }
       
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
                //步骤
                FlowStepModel flowModel = this.flowStepDAL.Get<FlowStepModel>(FKSN);

                //历史脚本 
                foreach (FlowStepHisModel hisModel in this.flowStepHisDAL.Query(FKSN))
                {
                    if (hisModel.SN == id)
                    {
                        hisModel.IsEnable = true;
                        string script = hisModel.Text;
                        if (script != "")
                        {
                            if (flowModel.CommandType == "Text") //如果是纯文本,则更新步骤内容
                            {
                                flowModel.CommandText = hisModel.Text;
                                this.flowStepDAL.Update(flowModel);
                            }
                            else //如果是存储过程
                            {
                                string procName = hisModel.FileName.Split(".")[0].Split("-")[0];
                                this.flowStepHisDAL.SmoExecute(flowModel.DataBase, procName, script);
                            }
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

 
    }
}
