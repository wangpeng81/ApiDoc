 
using ApiDoc.IDAL;
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
        public ParamDAL(ILogger<BaseDAL> logger, IDbHelper db) : base(logger, db)
        {
            base.tableName = base.GetTable(typeof(ParamModel));
        }

        public List<ParamModel> Query(int fksn)
        {
            List<ParamModel> list = new List<ParamModel>();

            try
            { 

                string strSql = string.Format("select * from {0} where FKSN ={1}", base.tableName, fksn);
                DataTable dt = db.FillTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                {
                    ParamModel info = new ParamModel();
                    base.CreateModel(info,dataRow);
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
 
    }
}
