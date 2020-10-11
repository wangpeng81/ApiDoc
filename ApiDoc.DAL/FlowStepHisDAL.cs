using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ApiDoc.DAL
{
    public class FlowStepHisDAL : BaseDAL, IFlowStepHisDAL
    {
       
        public FlowStepHisDAL(ILogger<BaseDAL> logger, IDbHelper db) : base(logger, db)
        {
            base.tableName = base.GetTable(typeof(FlowStepHisModel));
        }

        public List<FlowStepHisModel> Query(int FKSN)
        {
            string cmdText = "select SN,FKSN,FileName,IsEnable from " + base.tableName + " where fksn=" + FKSN + " order by IsEnable desc,DTime desc";
            DataTable dt = this.db.FillTable(cmdText);
            List<FlowStepHisModel> list = new List<FlowStepHisModel>();
            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepHisModel model = new FlowStepHisModel();
                base.CreateModel(model, dataRow);
                list.Add(model);
            }
            return list;
        }
    }
}
