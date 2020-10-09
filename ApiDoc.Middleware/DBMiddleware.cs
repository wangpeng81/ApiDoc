using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiDoc.Middleware
{
    public class DBMiddleware
    {

        private string SqlConnStr;

        private readonly RequestDelegate next;
        private readonly IDbHelper dbHelp;
        private readonly DBRouteValueDictionary routeDict;
        private HttpContext context;

        public DBMiddleware(RequestDelegate _next, 
                            IInterfaceDAL interfaceDAL, 
                            IFlowStepDAL flowStpeDAL,
                            IDbHelper dbHelp,
                            DBRouteValueDictionary _routeDict,
                            IConfiguration config)
        {
            this.next = _next;
            this.dbHelp = dbHelp;
            this.routeDict = _routeDict;

            SqlConnStr = config.GetConnectionString("SqlConnStr");
            
            //加载路由集合
            List< InterfaceModel > dtInterface = interfaceDAL.All();
            foreach (InterfaceModel model in dtInterface)
            {
                //加载步骤
                int SN = model.SN;
                string Url = model.Url;
                DBInterfaceModel dbInter = new DBInterfaceModel();
                dbInter.SerializeType = model.SerializeType;
                dbInter.Method = model.Method;
                dbInter.Steps =  flowStpeDAL.Query(SN);
                dbInter.IsTransaction = model.IsTransaction;
                dbInter.ExecuteType = model.ExecuteType;

                routeDict.Add(Url, dbInter);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            string path = context.Request.Path.ToString();

            RouteValueDictionary routeValue = context.Request.RouteValues;

            if (this.routeDict.ContainsKey(path))
            {
                await InvokeDB(context);
            }
            else
            {
                await next(context); 
            } 
        }

        private async Task InvokeDB(HttpContext context)
        {
            this.context = context; 

            string path = context.Request.Path.ToString();
            DBInterfaceModel response = this.routeDict[path];

            #region init

            SqlConnection connection = new SqlConnection(SqlConnStr);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            cmd.Connection = connection;
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = cmd;

            #endregion

            SqlTransaction tran = null;

            if (response.IsTransaction)
            {
                tran = connection.BeginTransaction();
                cmd.Transaction = tran;
            }

            DataResult returnValue = new DataResult();
            if (response.Steps.Count > 0)
            {
                #region 执行步骤
                foreach (FlowStepModel flow in response.Steps)
                {
                    switch (flow.CommandType)
                    {
                        case "Text":
                            cmd.CommandType = CommandType.Text;
                            break;
                        case "StoredProcedure":
                            cmd.CommandType = CommandType.StoredProcedure;
                            break;
                    }

                    if (flow.CommandText == "")
                    {
                        string msg = flow.StepName + " 没有数据库语句，请维护";
                        await InvokeException(returnValue,msg);
                        return;
                    }
                    cmd.CommandText = flow.CommandText;
                    cmd.Parameters.Clear();

                    if (response.Method.ToLower() == "get")
                    {
                        foreach (string key in context.Request.Query.Keys)
                        {
                            object value = context.Request.Query[key];
                            cmd.Parameters.AddWithValue(key, value);
                        }
                    }
                    else if (response.Method.ToLower() == "post")
                    {

                    }
                }
                #endregion

                #region 数据结果集类型
                try
                {
                    switch (response.ExecuteType)
                    {
                        case "Scalar":
                            returnValue = new DataResult();
                            returnValue.Result = cmd.ExecuteScalar();
                            break;
                        case "Int":
                            returnValue = new IntDataResult();
                            returnValue.Result = cmd.ExecuteNonQuery();
                            break;
                        case "DataSet":
                            returnValue = new DSDataResult();
                            DataSet ds = new DataSet();
                            sqlDA.Fill(ds);
                            returnValue.Result = ds;
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    string msg = ex.Message;
                    await InvokeException(returnValue, msg);
                    return;
                }
                
                #endregion

                if (response.IsTransaction)
                {
                    tran.Commit();
                }

                string resultValue = JsonHelper.SerializeJSON<DataResult>(returnValue);
                await context.Response.WriteAsync(resultValue);
            }
            else
            {
                await context.Response.WriteAsync(path + "没有任何步骤，请维护");
            }
        }

        private async Task InvokeException(DataResult returnValue, string exception) {

            returnValue.DataType = 1;
            returnValue.Exception = exception;
            string json = JsonHelper.SerializeJSON<DataResult>(returnValue);
            await this.context.Response.WriteAsync(json);
        }
    }
}
