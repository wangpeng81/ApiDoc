using ApiDoc.DAL.Interface;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    public class FlowStepDAL: BaseDAL, IFlowStepDAL
    { 
        public FlowStepDAL(ILogger<BaseDAL> logger) : base(logger)
        {

        }
 
        public List<FlowStepModel> Query(int fksn)
        {
            List<FlowStepModel> list = new List<FlowStepModel>();

            try
            {
                DbHelper db = new DbHelper();
                string tableName = base.GetTable(typeof(FlowStepModel));
                string strSql = string.Format("select * from {0} where FKSN= {1} order by StepOrder", tableName, fksn);
                DataTable dt = db.CreateSqlDataTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                {
                    FlowStepModel info = this.CreateObj(dataRow);
                    list.Add(info);
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError("DbCommand->api_interface=>Insert(s) 出错\r\n" + ex.Message);
                throw ex;
            }
            return list;
        }

        public int SaveCmdText(int SN, string CommandType, string CommandText)
        {
            try
            {
                if (CommandText == null)
                {
                    CommandText = "";
                }
                DbHelper db = new DbHelper();
                string strSql = "update api_flow_step  set CommandType = @CommandType, CommandText =@CommandText where SN = @SN";

                DbParameters p = new DbParameters();
                p.Add("SN", SN);
                p.Add("CommandType", CommandType);
                p.Add("CommandText", CommandText);
                int iResult = db.ExecuteSql(strSql, p);
                return iResult;
            }
            catch (Exception ex)
            {
                _logger.LogError("DbCommand->api_interface=>Insert(s) 出错\r\n" + ex.Message);
                throw ex;
            }

        }

        private FlowStepModel CreateObj(DataRow dataRow)
        {
            FlowStepModel info = new FlowStepModel();
            info.FKSN = int.Parse(dataRow["FKSN"].ToString());
            info.SN = int.Parse(dataRow["SN"].ToString());
            info.StepName = dataRow["StepName"].ToString();
            info.StepOrder = int.Parse(dataRow["StepOrder"].ToString()); 
            info.CommandText = dataRow["CommandText"].ToString();
            info.CommandType = dataRow["CommandType"].ToString();
            return info;
        }
    }
}
