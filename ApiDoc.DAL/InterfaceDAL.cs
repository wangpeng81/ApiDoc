using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Utility.Filter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    [Table("api_interface")]
    public class InterfaceDAL : BaseDAL, IInterfaceDAL
    {
        public InterfaceDAL(ILogger<BaseDAL> logger, IDbHelper db) :base(logger, db)
        {
            
        }

        public List<InterfaceModel> All()
        {
            List<InterfaceModel> list = new List<InterfaceModel>();
 
            string strSql = "select * from " + base.tableName;
            DataTable dt = db.FillTable(strSql);
            foreach (DataRow dataRow in dt.Rows)
            {
                InterfaceModel info = base.CreateModel<InterfaceModel>(dataRow);
                list.Add(info);
            }

            return list;
        }

        public List<InterfaceModel> All(string title, string url, int fksn)
        {
            List<InterfaceModel> list = new List<InterfaceModel>();

            try
            {
                string strSql = "with temp as( select * from api_folder where SN =" + fksn.ToString();
                strSql += " union all ";
                strSql += "select b.* from temp a inner join api_folder b on b.[ParentSN] = a.SN ) ";
                strSql += "select b.* from api_interface b inner join temp c on c.SN = b.FKSN";
                strSql += " and title like '%" + title + "%'";
                strSql += " and url like '%" + url + "%'";

                //所以数据
                if (fksn == 0)
                {
                    strSql = "select * from api_interface where title like '%" + title + "%'" + " and url like '%" + url + "%'"; 
                }

                DataTable dt = db.FillTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                { 
                    InterfaceModel info = base.CreateModel<InterfaceModel>(dataRow); 
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

        public string FullPath(int fksn)
        {
            try
            {
                string strSql = "with temp as";
                strSql += "(select * from api_folder where SN =" + fksn.ToString();
                strSql += "union all ";
                strSql += "select b.* from temp a inner join api_folder b on b.SN = a.ParentSN ) ";
                strSql += "select * from temp";

                DataTable dt = db.FillTable(strSql);

                string path = "";
                DataRow[] rows = dt.Select("ParentSN=0");
                if (rows.Length > 0)
                {
                    while (rows.Length > 0)
                    {
                        DataRow dataRow = rows[0];
                        path += "/" + dataRow["RoutePath"].ToString();

                        string SN = dataRow["SN"].ToString();
                        rows = dt.Select("ParentSN=" + SN);
                    } 
                }

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InterfaceModel> Query(bool isStop)
        {
            List<InterfaceModel> list = new List<InterfaceModel>();
            string vStop = isStop ? "1" : "0";
            string strSql = "select * from " + base.tableName + " where isStop = " + vStop;
            DataTable dt = db.FillTable(strSql);
            foreach (DataRow dataRow in dt.Rows)
            {
                InterfaceModel info = base.CreateModel<InterfaceModel>(dataRow);
                list.Add(info);
            }

            return list;
        }
    }
}
