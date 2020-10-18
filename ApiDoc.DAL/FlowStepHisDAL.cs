

using ApiDoc.IDAL;
using ApiDoc.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data; 

namespace ApiDoc.DAL
{
    [Table("api_flow_step_his")]
    public class FlowStepHisDAL : BaseDAL, IFlowStepHisDAL
    { 
        public FlowStepHisDAL(ILogger<BaseDAL> logger, 
            IDbHelper db) : base(logger, db)
        {
            
        }

        public List<FlowStepHisModel> Query(int FKSN)
        {
            string cmdText = "select * from " + base.tableName + " where fksn=" + FKSN + " order by IsEnable desc,DTime desc";
            DataTable dt = this.db.FillTable(cmdText);
            List<FlowStepHisModel> list = new List<FlowStepHisModel>();
            foreach (DataRow dataRow in dt.Rows)
            {
                FlowStepHisModel model = base.CreateModel<FlowStepHisModel>(dataRow); 
                list.Add(model);
            }
            return list;
        }

        public int SmoExecute(string database, string procName, string text)
        {
            //https://www.cnblogs.com/long-gengyun/archive/2012/05/25/2517954.html
            //http://www.smochen.com/detail?aid=1280 

            string connectonstring =  this.db.Configuration.GetConnectionString("ApiDocConnStr");
            Microsoft.Data.SqlClient.SqlConnection sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connectonstring);
            sqlConnection.Open();
            sqlConnection.ChangeDatabase(database);
             
            ServerConnection server1 = new ServerConnection(sqlConnection);
         
            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(server1);
            Microsoft.SqlServer.Management.Smo.Database db = server.Databases[database];

            if (db.StoredProcedures.Contains(procName))
            {
                text = text.Replace("CREATE PROCEDURE", "ALTER PROCEDURE", System.StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                text = text.Replace("ALTER PROCEDURE", "CREATE PROCEDURE", System.StringComparison.OrdinalIgnoreCase);
            }

            int i = server.ConnectionContext.ExecuteNonQuery(text);
            sqlConnection.Close();
            return 1;
        }
    }
}
