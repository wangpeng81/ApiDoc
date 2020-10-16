using ApiDoc.IBLL;
using ApiDoc.IDAL;
using ApiDoc.Models;
using System;

namespace ApiDoc.BLL
{
    public class InterfaceBLL : BaseBLL, IInterfaceBLL
    {
        private readonly IInterfaceDAL infterfaceDAL;
        private readonly IFlowStepDAL flowStepDAL;

        public InterfaceBLL(IInterfaceDAL infterfaceDAL, IFlowStepDAL flowStepDAL)
        {
            this.infterfaceDAL = infterfaceDAL;
            this.flowStepDAL = flowStepDAL;
        }

        public DBInterfaceModel GetInterfaceModel(int SN)
        {
            DBInterfaceModel dbInter = new DBInterfaceModel();
            InterfaceModel model = this.infterfaceDAL.Get<InterfaceModel>(SN);

            dbInter.SerializeType = model.SerializeType;
            dbInter.Method = model.Method;
            dbInter.IsTransaction = model.IsTransaction;
            dbInter.ExecuteType = model.ExecuteType;

            //步骤
            dbInter.Steps = this.flowStepDAL.Query(SN);

            return dbInter;
        }
    }
}
