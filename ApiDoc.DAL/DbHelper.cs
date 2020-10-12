 using System.Collections.Generic;
  using Microsoft.Extensions.Logging;
 
  using System.Data;
  using System;
  using System.Collections;
  using System.Reflection;
using System.Data.SqlClient;
using ApiDoc.IDAL;
using Microsoft.Extensions.Configuration;
using ApiDoc.Utility.Filter;

namespace ApiDoc.DAL
{
    public class DbHelper : IDbHelper
    {
        private SqlConnection connection;
        private SqlCommand cmd;
        private SqlDataAdapter sqlDA = new SqlDataAdapter();

        private string dsn;

        public DbHelper(IConfiguration config)
        { 
            string SqlConnStr = config.GetConnectionString("ApiDocConnStr");
            this.dsn = SqlConnStr;
            this.connection = new SqlConnection(SqlConnStr);
            this.cmd = new SqlCommand();
            this.cmd.CommandTimeout = 0;
            this.cmd.Connection = this.connection;
            this.sqlDA = new SqlDataAdapter();
            this.sqlDA.SelectCommand = this.cmd;
        }
          
        //IDbHelper
        public int ExecuteSql(string cmdText)
        {
            SqlCommand comd = CreateSqlComd(cmdText);
            return ToExecute(comd);
        }
        public int ExecuteSql(string cmdText, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(cmdText, p);
            return ToExecute(comd);
        }

        public object ExecuteScalar(string cmdText)
        {
            SqlCommand comd = CreateSqlComd(cmdText);
            return GetScalar(comd);
        }
        public object ExecuteScalar(string cmdText, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(cmdText, p);
            return GetScalar(comd);
        }

        public DataSet Fill(string cmdText)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(cmdText);
            return GetDataTable(adapter);
        } 
        public DataTable FillTable(string cmdText)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(cmdText);
            DataSet ds = GetDataTable(adapter);
            return ds.Tables[0];
        } 
        public DataSet Fill(string cmdText, DbParameters p)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(cmdText, p);
            return GetDataTable(adapter);
        }

        //private
        private void ConnOpen(ref SqlCommand comd)
        {
            if (comd.Connection.State == ConnectionState.Closed)
                comd.Connection.Open();
        }
        private void ConnClose(ref SqlCommand comd)
        {
            if (comd.Connection.State == ConnectionState.Open)
                comd.Connection.Close();
        }

        private SqlCommand CreateSqlComd(string cmdText)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlCommand comd = conn.CreateCommand();
                comd.CommandText = cmdText;
                comd.CommandType = CommandType.Text; 
                return comd;
            }
            catch (System.Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }
        private SqlCommand CreateSqlComd(string cmdText, DbParameters p)
        {
            try
            {
                SqlCommand comd = CreateSqlComd(cmdText);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        comd.Parameters.Add(p[i]);
                    }
                }
                return comd;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateSqlcomd(s,p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
      
        private int ToExecute(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                int iOk = comd.ExecuteNonQuery();
                ConnClose(ref comd);
                return iOk;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd); 
                throw new Exception(ex.Message);
            }
        }
        private object GetScalar(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                object o = comd.ExecuteScalar();
                ConnClose(ref comd);

                return o;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd); 
                throw new Exception(ex.Message);
            }
        }

        private SqlDataAdapter CreateSqlAdapter(string cmdText)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter apter = new SqlDataAdapter(cmdText, conn); 
                return apter;
            }
            catch (System.Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }
        private SqlDataAdapter CreateSqlAdapter(string cmdText, DbParameters p)
        {
            try
            {
                SqlDataAdapter apter = CreateSqlAdapter(cmdText);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        apter.SelectCommand.Parameters.Add(p[i]);
                    }
                }

                return apter;
            }
            catch (System.Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }

        private DataSet GetDataTable(SqlDataAdapter adapter)
        {
            try
            { 
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                return dt;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (adapter.SelectCommand.Connection.State == ConnectionState.Open)
                {
                    adapter.SelectCommand.Connection.Close();
                }
                adapter.Dispose();
            }
        }


        public SqlTransaction BeginTransaction()
        {
            SqlTransaction tran = this.connection.BeginTransaction();
            return tran;
        }
 
    }
}
                                                              