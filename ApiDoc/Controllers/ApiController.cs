using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDoc.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IFlowStepDAL flowStepDAL;
        private readonly IFlowStepHisDAL flowStepHisDAL;

        public ApiController(IFlowStepDAL flowStepDAL, IFlowStepHisDAL flowStepHisDAL)
        {
            this.flowStepDAL = flowStepDAL;
            this.flowStepHisDAL = flowStepHisDAL;
        }

        public string CS()
        { 
            return "apid";
        }

        [HttpPost]
        public int SmoExecute(List<int> ids)
        {
           

            return 1;
        }
    }
}
