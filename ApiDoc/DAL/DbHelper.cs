 using System.Collections.Generic;
  using Microsoft.Extensions.Logging;
 
  using System.Data;
  using System;
  using System.Collections;
  using System.Reflection;
using System.Data.SqlClient;

namespace ApiDoc.DAL
{
    public class DbHelper
    { 
    
        public DbHelper()
        {
            //dsn = Encrypt.Dec(dsn);  //解密 
            //dsn = Configuration.GetConnectionString("SqlDSN");
            dsn = UtilConf.GetConnectionString("SqlDSN");
        }

        public string dsn;
        public static DbHelper SqlDSN
        {
            get { return new DbHelper(); }
        }

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

        public SqlCommand CreateComd(string spName)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlCommand comd = conn.CreateCommand();
                comd.CommandText = spName;
                comd.CommandType = CommandType.StoredProcedure;

                return comd;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateComd(sp) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateComd(string spName, DbParameters p)
        {
            try
            {
                SqlCommand comd = CreateComd(spName);
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
                //Log.LogError("DbCommand->CreateComd(sp) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateSqlComd(string strSql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlCommand comd = conn.CreateCommand();
                comd.CommandText = strSql;
                comd.CommandType = CommandType.Text;

                return comd;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateSqlComd(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateSqlComd(string strSql, DbParameters p)
        {
            try
            {
                SqlCommand comd = CreateSqlComd(strSql);

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
   
        public SqlDataAdapter CreateAdapter(string spName)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter comdAdapter = new SqlDataAdapter(spName, conn);
                comdAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                return comdAdapter;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateAdapter(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public SqlDataAdapter CreateAdapter(string spName, DbParameters p)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter comdAdapter = new SqlDataAdapter(spName, conn);
                comdAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        comdAdapter.SelectCommand.Parameters.Add(p[i]);
                    }
                }

                return comdAdapter;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateAdapter(s, p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public SqlDataAdapter CreateSqlAdapter(string strSql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter apter = new SqlDataAdapter(strSql, conn);
                apter.SelectCommand.CommandType = CommandType.Text;

                return apter;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->CreateSqlAdapter(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public SqlDataAdapter CreateSqlAdapter(string strSql, DbParameters p)
        {
            try
            {
                SqlDataAdapter apter = CreateSqlAdapter(strSql);

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
                //Log.LogError("DbCommand->CreateSqlAdapter(s,p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
   
        public SqlDataReader CreateDataReader(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return GetDataReader(comd);
        } 
        public SqlDataReader CreateDataReader(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return GetDataReader(comd);
        } 
        public SqlDataReader CreateSqlDataReader(string strSql)
        {
            SqlCommand comd = CreateSqlComd(strSql);
            return GetDataReader(comd);
        }
       
        public SqlDataReader CreateSqlDataReader(string strSql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(strSql, p);
            return GetDataReader(comd);
        }
        private SqlDataReader GetDataReader(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                return comd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->GetDataReader() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        
        public DataTable CreateDataTable(string spName)
        {
            SqlDataAdapter adapter = CreateAdapter(spName);
            return GetDataTable(adapter);
        } 
        public DataTable CreateDataTable(string spName, DbParameters p)
        {
            SqlDataAdapter adapter = CreateAdapter(spName, p);
            return GetDataTable(adapter);
        }
        
        public DataTable CreateSqlDataTable(string strSql)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(strSql);
            return GetDataTable(adapter);
        }
        
        public DataTable CreateSqlDataTable(string strSql, DbParameters p)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(strSql, p);
            return GetDataTable(adapter);
        }

        private DataTable GetDataTable(SqlDataAdapter adapter)
        {
            try
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (System.Exception ex)
            {
                //Log.LogError("DbCommand->GetSqlDataTable() 出错\r\n" + ex.Message);
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
 
        public object CreateScalar(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return GetScalar(comd);
        }
        
        public object CreateScalar(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return GetScalar(comd);
        }
        
        public object CreateSqlScalar(string strSql)
        {
            SqlCommand comd = CreateSqlComd(strSql);
            return GetScalar(comd);
        }
        
        public object CreateSqlScalar(string strSql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(strSql, p);
            return GetScalar(comd);
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
                //Log.LogError("DbCommand->GetScalar() 出错\r\n" + ex.Message);
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
                //Log.LogError("DbCommand->ToExecute() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private int ToExecuteInt(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                int iOk = 0;
                int.TryParse(comd.ExecuteScalar().ToString(), out iOk);
                ConnClose(ref comd);
                return iOk;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ToExecute() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
  
        
        public int Execute(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return ToExecute(comd);
        } 
        public int Execute(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return ToExecute(comd);
        } 
        public int ExecuteSql(string sql)
        {
            SqlCommand comd = CreateSqlComd(sql);
            return ToExecute(comd);
        }
       
        public int ExecuteSqlInt(string sql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            return ToExecuteInt(comd);
        }
        public int ExecuteSql(string sql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            return ToExecute(comd);
        } 
        public string ExecuteOut(string spName, DbParameters p, string outParamName)
        {
            SqlCommand comd = CreateComd(spName, p);
            //comd.Parameters.Add(new SqlParameter(outParamName, SqlDbType.VarChar, 50));
            //comd.Parameters[outParamName].Direction = ParameterDirection.Output;

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[outParamName].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteOut() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public string ExecuteOut(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter("@Result", SqlDbType.VarChar, 50));
            comd.Parameters["@Result"].Direction = ParameterDirection.Output;

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["@Result"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteOut() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public string ExecuteReturn(string spName, DbParameters p, string retParam)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter(retParam, SqlDbType.VarChar, 50));
            comd.Parameters[retParam].Direction = ParameterDirection.ReturnValue;
 
            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[retParam].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public string ExecuteReturn(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.VarChar, 50));
            comd.Parameters["ReturnValue"].Direction = ParameterDirection.ReturnValue;

            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["ReturnValue"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        } 
        public string ExecuteSqlReturn(string sql, DbParameters p, string retParam)
        {
                    SqlCommand comd = CreateSqlComd(sql, p);
                    comd.Parameters.Add(new SqlParameter(retParam, SqlDbType.VarChar, 50));
                    comd.Parameters[retParam].Direction = ParameterDirection.ReturnValue;
          
            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[retParam].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public string ExecuteSqlReturn(string sql, DbParameters p)
        {
                       SqlCommand comd = CreateSqlComd(sql, p);
                       comd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.VarChar, 50));
                       comd.Parameters["ReturnValue"].Direction = ParameterDirection.ReturnValue;
            
            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["ReturnValue"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                //Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

    }
                                                                   }
                                                              