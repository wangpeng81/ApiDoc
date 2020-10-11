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
            base.tableName = base.GetTable(typeof(FlowStepHis));
        }

        public List<FlowStepHis> Query(int FKSN)
        {
            string cmdText = "select SN,FKSN,FileName,IsEnable from " + base.tableName + " where fksn=" + FKSN + " order by IsEnable desc";
            DataTable dt = this.db.FillTable(cmdText);
            List<FlowStepHis> list = new List<FlowStepHis>();
            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepHis model = new FlowStepHis();
                base.CreateModel(model, dataRow);
                list.Add(model);
            }
            return list;
        }
    }
}
