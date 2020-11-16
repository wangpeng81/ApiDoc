using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.IDAL
{
    public interface IFlowStepDAL : IBaseDAL
    {
        List<FlowStepModel> Query(int fksn);
        List<FlowStepModel> QueryOfParam(int fksn);
        int SaveCmdText(int SN, string CommandType, string CommandText, string DataBase, string ServiceName);
        int DeleteAll(int SN);
    }
}
