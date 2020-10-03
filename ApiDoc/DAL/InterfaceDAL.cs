using ApiDoc.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.DAL
{
    public class InterfaceDAL : BaseDAL, IInterfaceDAL
    {
        public InterfaceDAL(ILogger<BaseDAL> logger):base(logger)
        {
             
        }
 
        public List<InterfaceModel> All()
        {
            List<InterfaceModel> list = new List<InterfaceModel>();

            try
            {
                DbHelper db = new DbHelper();
                string strSql = "select * from api_interface";
                DataTable dt = db.CreateSqlDataTable(strSql);

                DataRow[] rows = dt.Select("FKSN=0");
                if (rows.Length > 0)
                {
                    foreach (DataRow dataRow in rows)
                    {
                        InterfaceModel info = new InterfaceModel();
                        info.FKSN = int.Parse(dataRow["FKSN"].ToString());
                        info.SN = int.Parse(dataRow["SN"].ToString());
                        info.Url = dataRow["Url"].ToString();
                        info.Title = dataRow["Title"].ToString();
                        info.Method = dataRow["Method"].ToString();
                        //info.ProcName = dataRow["ProcName"].ToString();
                        list.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DbCommand->api_interface=>Insert(s) 出错\r\n" + ex.Message);
                throw ex;
            }
            return list;
        }

        public List<InterfaceModel> All(string title, string url, int fksn)
        {
            List<InterfaceModel> list = new List<InterfaceModel>();

            try
            {
                DbHelper db = new DbHelper();
                string strSql = "select * from api_interface where FKSN=" + fksn.ToString() 
                             + " and title like '%" + title + "%'"
                             + " and url like '%" + url + "%'";
                DataTable dt = db.CreateSqlDataTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                {
                    InterfaceModel info = new InterfaceModel();
                    info.FKSN = int.Parse(dataRow["FKSN"].ToString());
                    info.SN = int.Parse(dataRow["SN"].ToString());
                    info.Url = dataRow["Url"].ToString();
                    info.Title = dataRow["Title"].ToString();
                    info.Method = dataRow["Method"].ToString();
                    //info.ProcName = dataRow["ProcName"].ToString();
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
