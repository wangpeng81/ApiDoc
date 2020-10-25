using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.IDAL
{
    public interface IFlowStepParamDAL : IBaseDAL
    {
        List<FlowStepParamModel> Query(int FKSN);
    }
}
