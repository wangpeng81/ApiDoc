using System; 
using ApiDoc.IDAL;
using ApiDoc.Middleware;
using ApiDoc.Models;
using ApiDoc.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiDoc.Controllers
{ 
    /// <summary>
    /// 路由注册
    /// </summary>
     
    public class RouteValueController : ControllerBase
    {
        private readonly IInterfaceDAL infterfaceDAL;       //接口
        private readonly IFlowStepDAL flowStepDAL;          //步骤
        private readonly DBRouteValueDictionary routeDict;  //路由集合

        public RouteValueController(IInterfaceDAL infterfaceDAL, IFlowStepDAL flowStepDAL, DBRouteValueDictionary routeDict)
        {
            this.infterfaceDAL = infterfaceDAL;
            this.flowStepDAL = flowStepDAL;
            this.routeDict = routeDict;
        }

        [HttpGet]
        public myResponse UpLoad(int SN)
        {
            myResponse res = new myResponse();

            InterfaceModel model = new InterfaceModel();
            model.SN = SN;
            model = (InterfaceModel)this.infterfaceDAL.Get(model);

            DBInterfaceModel dbInter = new DBInterfaceModel();
            dbInter.SerializeType = model.SerializeType;
            dbInter.Method = model.Method;
            dbInter.IsTransaction = model.IsTransaction;
            dbInter.ExecuteType = model.ExecuteType;

            //步骤
            dbInter.Steps = this.flowStepDAL.Query(SN);

            string url = model.Url;
            if (this.routeDict.ContainsKey(url))
            {
                this.routeDict[url] = dbInter;
            }
            else
            {
                this.routeDict.Add(url, dbInter);
            }
            return res;
        }
 
        public myResponse Delete(string Url)
        {
            myResponse res = new myResponse();

            try
            {
                if (!this.routeDict.ContainsKey(Url))
                {
                    res.DataType = 1;
                    res.Exception = "不存在,无法删除!";
                }
                else
                {
                    this.routeDict.Remove(Url);
                } 
            }
            catch (Exception ex)
            {
                res.DataType = 100;
                res.Exception = ex.Message; 
            }
           
            return res;
        }
    }
}
