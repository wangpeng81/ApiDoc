using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace ApiDoc.IDAL
{
    public interface IDbHelper
    {
        object ExecuteScalar(string cmdText);
        object ExecuteScalar(string cmdText, DbParameters p);

        int ExecuteSql(string cmdText);
        int ExecuteSql(string cmdText, DbParameters p);
         
        DataSet Fill(string cmdText);
        DataSet Fill(string cmdText, DbParameters p);

        DataTable FillTable(string cmdText);

        SqlTransaction BeginTransaction(); 
    }
}
