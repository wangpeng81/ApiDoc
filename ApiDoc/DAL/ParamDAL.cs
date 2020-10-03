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
    public class ParamDAL : BaseDAL, IParamDAL
    {
        public ParamDAL(ILogger<BaseDAL> logger):base(logger)
        { 

        }

        public List<ParamModel> Query(int fksn)
        {
            List<ParamModel> list = new List<ParamModel>();

            try
            {
                DbHelper db = new DbHelper();
                string strSql = "select * from api_interface_param where FKSN=" + fksn.ToString() + " order by StepOrder";
                DataTable dt = db.CreateSqlDataTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                {
                    ParamModel info = this.CreateObj(dataRow);
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

        private ParamModel CreateObj(DataRow dataRow)
        {
            ParamModel info = new ParamModel();
            info.FKSN = int.Parse(dataRow["FKSN"].ToString());
            info.SN = int.Parse(dataRow["SN"].ToString());
            info.ParamName = dataRow["ParamName"].ToString();
            info.ParamType = dataRow["ParamType"].ToString();
            info.DefaultValue = dataRow["DefaultValue"].ToString();
            return info;
        }
    }
}
