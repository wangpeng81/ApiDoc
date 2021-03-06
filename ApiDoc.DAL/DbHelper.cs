﻿ using System.Collections.Generic; 
 using System.Data;
 using System;  
using ApiDoc.IDAL;
using Microsoft.Extensions.Configuration; 
using Autofac;
 
namespace ApiDoc.DAL
{
    public class DbHelper : IDbHelper
    {
        private readonly IConfiguration config;
        private readonly IComponentContext componentContext;
        private readonly IDbConnection dbConnection;
         
        public IConfiguration Configuration => config;

        public DbHelper(IConfiguration config, IComponentContext componentContext)
        { 
            string SqlConnStr = config.GetConnectionString("ApiDocConnStr");
            this.config = config;
            this.componentContext = componentContext;
            this.dbConnection = componentContext.ResolveNamed<IDbConnection>("SqlServer");
            this.dbConnection.ConnectionString = SqlConnStr; 
        }
          
        //IDbHelper
        public int ExecuteSql(string cmdText)
        {
            IDbCommand comd = CreateSqlComd(cmdText);
            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }

            int iOk = comd.ExecuteNonQuery();
            ((IDisposable)comd).Dispose();
            return iOk;
        }
        public int ExecuteSql(string cmdText, DbParameters p)
        {
            IDbCommand comd = CreateSqlComd(cmdText, p);
            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }
            int iOk = comd.ExecuteNonQuery();
            ((IDisposable)comd).Dispose();
            return iOk;
        }

        public object ExecuteScalar(string cmdText)
        {
            IDbCommand comd = CreateSqlComd(cmdText);
            object o = comd.ExecuteScalar();
            ((IDisposable)comd).Dispose();
            return o;
        }
        public object ExecuteScalar(string cmdText, DbParameters p)
        {
            IDbCommand comd = CreateSqlComd(cmdText, p);
            object o = comd.ExecuteScalar();
            ((IDisposable)comd).Dispose();
            return o;
        }

        public DataSet Fill(string cmdText)
        {
            IDbDataAdapter adapter = CreateSqlAdapter(cmdText);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            ((IDisposable)adapter).Dispose();
            return ds;
        } 
        public DataTable FillTable(string cmdText)
        {
            IDbDataAdapter adapter = CreateSqlAdapter(cmdText);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            ((IDisposable)adapter).Dispose();
            return ds.Tables[0];
        } 

        public DataSet Fill(string cmdText, DbParameters p)
        {
            IDbDataAdapter adapter = CreateSqlAdapter(cmdText, p);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }

        public IDbTransaction BeginTransaction()
        {
            IDbTransaction tran = this.dbConnection.BeginTransaction(); 
            return tran;
        }
 
        private IDbCommand CreateSqlComd(string cmdText)
        {
            try
            { 
                IDbCommand comd = this.dbConnection.CreateCommand();
                comd.CommandText = cmdText;
                comd.CommandType = CommandType.Text;  
                return comd;
            }
            catch (System.Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }
        private IDbCommand CreateSqlComd(string cmdText, DbParameters p)
        {
            try
            {
                IDbCommand comd = CreateSqlComd(cmdText);

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
                throw new Exception(ex.Message);
            }
        }
       
        
        private IDbDataAdapter CreateSqlAdapter(string cmdText)
        {
           
                IDbDataAdapter dataAdapter = componentContext.ResolveNamed<IDbDataAdapter>("SqlServer");
                IDbCommand dbCommand = this.dbConnection.CreateCommand();
                dbCommand.CommandText = cmdText;
                dbCommand.Connection = this.dbConnection;
                dataAdapter.SelectCommand = dbCommand; 
                return dataAdapter; 
        }
        private IDbDataAdapter CreateSqlAdapter(string cmdText, DbParameters p)
        {
            try
            {
                IDbDataAdapter apter = CreateSqlAdapter(cmdText);

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
 
        public void Open()
        {
            this.dbConnection.Open();
        }

        public void Close()
        {
            this.dbConnection.Close(); 
        }
    }
}
                                                              