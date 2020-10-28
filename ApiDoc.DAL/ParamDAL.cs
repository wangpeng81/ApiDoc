 
using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    [Table("api_interface_param")]
    public class ParamDAL : BaseDAL, IParamDAL
    {
        public ParamDAL(IDbHelper db) : base(db)
        {
            
        }

        public List<ParamModel> Query(int fksn)
        {
            List<ParamModel> list = new List<ParamModel>();

            string strSql = string.Format("select * from {0} where FKSN ={1}", base.tableName, fksn);
            DataTable dt = db.FillTable(strSql);

            foreach (DataRow dataRow in dt.Rows)
            {
                ParamModel info = base.CreateModel<ParamModel>(dataRow);
                list.Add(info);
            }

            return list;
        }
 
    }
}
