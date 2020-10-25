using ApiDoc.IBLL;
using ApiDoc.IDAL;
using ApiDoc.Models;
using System;
using System.Collections.Generic;

namespace ApiDoc.BLL
{
    public class InterfaceBLL : BaseBLL, IInterfaceBLL
    {
        private readonly IInterfaceDAL infterfaceDAL;
        private readonly IParamDAL paramDAL;
        private readonly IFlowStepDAL flowStepDAL;

        public InterfaceBLL(IInterfaceDAL infterfaceDAL,
                            IParamDAL paramDAL,
                            IFlowStepDAL flowStepDAL)
        {
            this.infterfaceDAL = infterfaceDAL;
            this.paramDAL = paramDAL;
            this.flowStepDAL = flowStepDAL;
        }

        public DBInterfaceModel GetInterfaceModel(int SN)
        {
          
            InterfaceModel model = this.infterfaceDAL.Get<InterfaceModel>(SN);
            DBInterfaceModel dbInter = CreateModel(model); 
            return dbInter;
        }
 
        private DBInterfaceModel CreateModel(InterfaceModel model)
        {
            DBInterfaceModel dbInter = new DBInterfaceModel();
            dbInter.SerializeType = model.SerializeType;
            dbInter.Method = model.Method;
            dbInter.IsTransaction = model.IsTransaction;
            dbInter.ExecuteType = model.ExecuteType;
            dbInter.Steps = this.flowStepDAL.Query(model.SN);

            //接口参数
            string auth = "";
            foreach (ParamModel param in this.paramDAL.Query(model.SN))
            {
                if (auth != "")
                {
                    auth += "_";
                }
                auth += param.ParamName;
            }
            auth = auth.ToLower();
            dbInter.Auth = auth;
            return dbInter;
        }
    }
}
