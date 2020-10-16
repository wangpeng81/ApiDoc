using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ApiDoc.DAL
{
    [Table("api_flow_step_his")]
    public class FlowStepHisDAL : BaseDAL, IFlowStepHisDAL
    {
       
        public FlowStepHisDAL(ILogger<BaseDAL> logger, IDbHelper db) : base(logger, db)
        {
             
        }

        public List<FlowStepHisModel> Query(int FKSN)
        {
            string cmdText = "select SN,FKSN,FileName,IsEnable from " + base.tableName + " where fksn=" + FKSN + " order by IsEnable desc,DTime desc";
            DataTable dt = this.db.FillTable(cmdText);
            List<FlowStepHisModel> list = new List<FlowStepHisModel>();
            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepHisModel model = base.CreateModel<FlowStepHisModel>(dataRow); 
                list.Add(model);
            }
            return list;
        }
 
        public int RunProcSql(List<int> ids)
        {
            return 0;
        }
    }
}
