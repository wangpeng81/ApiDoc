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

        public ParamController(IParamDAL paramDAL)
        {
            this.paramDAL = paramDAL;
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

    }
}
