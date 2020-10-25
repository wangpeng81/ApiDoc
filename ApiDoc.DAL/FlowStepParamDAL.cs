using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace ApiDoc.DAL
{
    [Table("api_flow_step_param")]
    public class FlowStepParamDAL : BaseDAL, IFlowStepParamDAL
    {
        public FlowStepParamDAL(ILogger<FlowStepParamDAL> logger,
            IDbHelper db) : base(logger, db)
        {

        }

        public List<FlowStepParamModel> Query(int FKSN)
        {
            string cmdText = "select * from " + base.tableName + " where fksn =" + FKSN.ToString();
            DataTable dt = base.db.FillTable(cmdText);

            List<FlowStepParamModel> list = new List<FlowStepParamModel>();
            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepParamModel model = base.CreateModel<FlowStepParamModel>(dataRow);
                list.Add(model);
            }
            return list;
        }
    }
}
