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
    public class InterfaceDAL : BaseDAL, IInterfaceDAL
    {
        public InterfaceDAL(ILogger<BaseDAL> logger, IDbHelper db) :base(logger, db)
        {
             
        }
 
        public List<InterfaceModel> All()
        {
            List<InterfaceModel> list = new List<InterfaceModel>();

            try
            {
                string strSql = "select * from api_interface";
                DataTable dt = db.FillTable(strSql); 
                foreach (DataRow dataRow in dt.Rows)
                {
                    InterfaceModel info = new InterfaceModel();
                    base.CreateModel(info, dataRow);
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

        public List<InterfaceModel> All(string title, string url, int fksn)
        {
            List<InterfaceModel> list = new List<InterfaceModel>();

            try
            { 
                string strSql = "select * from api_interface where FKSN=" + fksn.ToString() 
                             + " and title like '%" + title + "%'"
                             + " and url like '%" + url + "%'";
                DataTable dt = db.FillTable(strSql);

                foreach (DataRow dataRow in dt.Rows)
                { 
                    InterfaceModel info = new InterfaceModel();
                    base.CreateModel(info, dataRow);
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
