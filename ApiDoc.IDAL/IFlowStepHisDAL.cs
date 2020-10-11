using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.IDAL
{
    public interface IFlowStepHisDAL : IBaseDAL
    {
        List<FlowStepHis> Query(int FKSN);
    }
}
