﻿using ApiDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL.Interface
{
    public interface IFlowStepDAL : IBaseDAL
    {
        List<FlowStepModel> Query(int fksn);
        int SaveCmdText(int SN, string CommandType, string CommandText);
    }
}
