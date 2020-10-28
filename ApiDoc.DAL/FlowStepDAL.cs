 
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    [Table("api_flow_step")]
    public class FlowStepDAL: BaseDAL, IFlowStepDAL
    {
     
        public FlowStepDAL(IDbHelper db) : base(db)
        {
            
        }

        public int DeleteAll(int SN)
        {
            string vSN = SN.ToString();
            string cmdText = "delete from api_flow_step where SN = " + vSN;
            cmdText += ";delete from api_flow_step_param where fksn = " + vSN;
            cmdText += ";delete from api_flow_step_his where fksn = " + vSN;
            int result = db.ExecuteSql(cmdText);
            return result;
        }

        public List<FlowStepModel> Query(int fksn)
        {
            List<FlowStepModel> list = new List<FlowStepModel>();
             
                string strSql = string.Format("select * from {0} where FKSN= {1} order by StepOrder", tableName, fksn);
                DataTable dt = db.FillTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                {
                    FlowStepModel info = this.CreateModel<FlowStepModel>(dataRow);
                    list.Add(info);
                } 
            
            return list;
        }

        public List<FlowStepModel> QueryOfParam(int fksn)
        {
            List<FlowStepModel> list = new List<FlowStepModel>();

            string strSql = string.Format("select * from {0} where FKSN= {1} order by StepOrder", tableName, fksn);
            DataTable dt = db.FillTable(strSql);

            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepModel info = this.CreateModel<FlowStepModel>(dataRow);

                string strSql1 = "select * from api_flow_step_param where fksn =" + info.SN.ToString();
                info.Params = new List<FlowStepParamModel>();

                foreach (DataRow dataRow1 in db.FillTable(strSql1).Rows)
                {
                    FlowStepParamModel model1 = base.CreateModel<FlowStepParamModel>(dataRow1);
                    info.Params.Add(model1);
                } 

                list.Add(info);
            }

            return list;
        }

        public int SaveCmdText(int SN, string CommandType, string CommandText, string DataBase)
        {
            
                if (CommandText == null)
                {
                    CommandText = "";
                } 
                string strSql = "update "+this.tableName+ "  set CommandType = @CommandType, CommandText =@CommandText, [DataBase] = @DataBase where SN = @SN";

                DbParameters p = new DbParameters();
                p.Add("SN", SN);
                p.Add("CommandType", CommandType);
                p.Add("CommandText", CommandText);
                p.Add("DataBase", DataBase);
                int iResult = db.ExecuteSql(strSql, p);
                return iResult;
            
        }
 
    }
}
