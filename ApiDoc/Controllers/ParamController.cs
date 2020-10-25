using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiDoc.Controllers
{
    public class ParamController : Controller
    {
        private readonly IParamDAL paramDAL;
        private readonly IFlowStepParamDAL flowStepParamDAL;

        public ParamController(IParamDAL paramDAL, IFlowStepParamDAL flowStepParamDAL)
        {
            this.paramDAL = paramDAL;
            this.flowStepParamDAL = flowStepParamDAL;
        }

        public IActionResult Index()
        {
            return View();
        }

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


        #region 步骤参数

        [HttpPost]
        public IActionResult StepParamList(int FKSN)
        {
            ViewData.Add("FKSN", FKSN);

            List<FlowStepParamModel> list = this.flowStepParamDAL.Query(FKSN);
            return PartialView("/Views/Interface/FlowStepParamList.cshtml", list);
        }

        [HttpPost]
        public IActionResult StepParamSave(FlowStepParamModel model)
        {
            if (model.SN > 0)
            {
                this.flowStepParamDAL.Update(model);
            }
            else
            {
                this.flowStepParamDAL.Insert(model);
            }
            return StepParamList(model.FKSN);
        }

        [HttpPost]
        public IActionResult StepParamDelete(List<int> ids, int FKSN)
        {
            foreach (int SN in ids)
            {
                FlowStepParamModel model = new FlowStepParamModel();
                model.SN = SN;
                int i = this.flowStepParamDAL.Delete(model);
            } 
            return StepParamList(FKSN);
        }
         
        [HttpPost]
        public IActionResult InterParamList(int FKSN)
        {
            List<ParamModel> list = this.paramDAL.Query(FKSN);
            return PartialView("/Views/Interface/ParamSelectList.cshtml", list);
        }

        [HttpPost]
        public IActionResult StepParamSaveList(List<FlowStepParamModel> list)
        {
            int fksn = 0;
            foreach (FlowStepParamModel model in list)
            {
                fksn = model.FKSN;
                model.IsPreStep = false;
                this.flowStepParamDAL.Insert(model);
            }

            return StepParamList(fksn);
        }

        #endregion

    }
}
