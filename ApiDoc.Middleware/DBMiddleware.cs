using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic; 
using System.Data; 
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Reflection;
using System.Text;  
using System.Threading.Tasks;

using System;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ApiDoc.IBLL;

namespace ApiDoc.Middleware
{
    public class DBMiddleware
    {

        private string SqlConnStr;

        private readonly RequestDelegate next;
        private readonly IParamDAL paramDAL;
        private readonly IDbHelper dbHelp;
        private readonly DBRouteValueDictionary routeDict;
        private readonly ILogger<DBMiddleware> logger;
        private HttpContext context;

        private string ServerIP = "";
        private string pwd = "";

        public DBMiddleware(RequestDelegate _next,
                            IInterfaceDAL interfaceDAL,
                            IFlowStepDAL flowStepDAL,
                            IParamDAL paramDAL,
                            IDbHelper dbHelp,
                            DBRouteValueDictionary _routeDict,
                            IConfiguration config,
                            ILogger<DBMiddleware> logger)
        {
            this.next = _next;
            this.paramDAL = paramDAL;
            this.dbHelp = dbHelp;
            this.routeDict = _routeDict;
            this.logger = logger;
            SqlConnStr = config.GetConnectionString("ApiDocConnStr");
            this.ServerIP = config.GetConnectionString("ServerIP");
            this.pwd = config.GetConnectionString("pwd");

            //加载路由集合

            List<InterfaceModel> dtInterface = interfaceDAL.Query(false);
            foreach (InterfaceModel model in dtInterface)
            {
                //加载步骤
                int SN = model.SN;
                string Url = model.Url;
                DBInterfaceModel dbInter = new DBInterfaceModel();
                dbInter.SerializeType = model.SerializeType;
                dbInter.Method = model.Method;
                dbInter.Steps = flowStepDAL.QueryOfParam(SN);
                dbInter.IsTransaction = model.IsTransaction;
                dbInter.ExecuteType = model.ExecuteType;

                //接口参数
                string auth = ""; 
                foreach (ParamModel param in this.paramDAL.Query(SN))
                {
                    if (auth != "")
                    {
                        auth += "_";
                    }
                    auth += param.ParamName;
                }
                auth = auth.ToLower();
                dbInter.Auth = auth;

                if (!routeDict.ContainsKey(Url))
                {
                    routeDict.Add(Url, dbInter);
                }
            }

            logger.LogInformation("加载接口完成,共" + dtInterface.Count.ToString());

        }

        public async Task Invoke(HttpContext context)
        {
            this.context = context;
            string path = context.Request.Path.ToString();
            this.logger.LogInformation(path);
            switch (path)
            {
                case "/CS":
                    await this.WriteAsync("欢迎浏览ApiDoc");
                    return;
                case "/CSDB":
                    return;
            }

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

            string exceMsg = ""; //存储异常的存储过程名  
            if (response.Steps.Count > 0)
            {
                //参数验证
                //从Request中获取参数集合
                Dictionary<string, object> dict = null;
                string method = response.Method.ToLower();
                string auth = "";
                if (method == "post")
                {
                    var reader = new StreamReader(context.Request.Body);
                    var contentFromBody = reader.ReadToEnd();
                    if (contentFromBody != "")
                    {
                        dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentFromBody);
                    }

                    foreach (KeyValuePair<string, object> kv in dict)
                    {
                        if (auth != "")
                        {
                            auth += "_";
                        }
                        auth += kv.Key;
                    } 
                }
                else if (method == "get")
                {
                    dict = new Dictionary<string, object>();
                    foreach (KeyValuePair<string, StringValues> kv in context.Request.Query)
                    {
                        if (!dict.ContainsKey(kv.Key))
                        {
                            if (auth != "")
                            {
                                auth += "_";
                            } 
                            auth += kv.Key; 
                            dict.Add(kv.Key, kv.Value[0]);
                        } 
                    }
                }
                else
                {
                    await this.InvokeException("此平台只支持post,get");
                    return;
                }

                auth = auth.ToLower();
                if (response.Auth != auth)
                {
                    await this.InvokeException("接收参数[" + response.Auth + "]与规则["+auth+"]不匹配" );
                    return;
                }

                SqlConnection connection = new SqlConnection(SqlConnStr);
                SqlTransaction tran = null;
                try
                {
                    connection.Open();

                    if (response.IsTransaction)
                    {
                        tran = connection.BeginTransaction();
                    }

                    //执行第一步 
                    string text = this.InvokeFistStep(response, connection, tran, dict, out exceMsg);

                    if (exceMsg != "")
                    {
                        tran.Rollback();
                        await this.InvokeException(exceMsg);
                        return;
                    }

                    if (response.IsTransaction)
                    {
                        tran.Commit();
                    }

                    await this.WriteAsync(text);
                }
                catch (System.Exception ex)
                {
                    if (response.IsTransaction)
                    {
                        tran.Rollback();
                    } 
                    await this.InvokeException(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            { 
                await this.WriteAsync(path + "没有任何步骤，请维护");
            }
        }

        private string InvokeFistStep(DBInterfaceModel response, SqlConnection connection, SqlTransaction tran, Dictionary<string, object> dict, out string exceMsg)
        {
            exceMsg = "";

            DataRow dataPre = null;
            string result = "";
            int rowIndex = 0;
            int count = response.Steps.Count; 
            foreach (FlowStepModel step in response.Steps)
            {
                if (step.CommandText == "")
                {
                    exceMsg = step.StepName + " 没有数据库语句，请维护";
                    return "";
                }

                connection.ChangeDatabase(step.DataBase);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                cmd.Connection = connection;

                if (response.IsTransaction)
                {
                    cmd.Transaction = tran;
                }

                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = cmd;

                //初始化参数
                switch (step.CommandType)
                {
                    case "Text":
                        cmd.CommandType = CommandType.Text;
                        break;
                    case "StoredProcedure":
                        cmd.CommandType = CommandType.StoredProcedure;
                        break;
                }
                cmd.CommandText = step.CommandText;
                cmd.Parameters.Clear();
                bool bOK = CreateParam(response, cmd, dataPre, step, dict, out exceMsg);
                if (!bOK)
                {
                    return "";
                }

                try
                {
                    //执行Sql
                    if (rowIndex == count - 1) //最后一步
                    {
                        result = this.ExecSql(response, connection, sqlDA, cmd);
                    }
                    else
                    {
                        //如果不是最后一步，存储当前步结果
                        DataTable dt = new DataTable();
                        sqlDA.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            dataPre = dt.Rows[0];
                        }
                    }

                }
                catch (Exception ex)
                {
                    string message = response.Url　+ ">>" + step.StepName + ">>" + ex.Message;
                    this.logger.LogError(message);
                    throw new Exception(message);
                }
                
                rowIndex++;
            } 
            return result;
        }
 
        private string ExecSql(DBInterfaceModel response, SqlConnection connection, SqlDataAdapter sqlDA, SqlCommand cmd)
        {
            string json = "";
            XmlHelper xmlHelp = new XmlHelper();

            object objResutl = new object();
            switch (response.ExecuteType)
            {
                case "Scalar":
                    objResutl = cmd.ExecuteScalar();
                    break;
                case "Int":
                    objResutl = cmd.ExecuteNonQuery();
                    break;
                case "DataSet": 
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);
                    objResutl = ds;
                    break;
            }

            if (response.SerializeType == "Xml")
            {
                json = xmlHelp.SerializeXML(objResutl);
            }
            else if (response.SerializeType == "Json")
            {
                json = JsonConvert.SerializeObject(objResutl);
            }
            else
            {
                json = objResutl.ToString();
            }
            return json;
        }

        private bool CreateParam(DBInterfaceModel response, SqlCommand cmd, DataRow dataPre, FlowStepModel step, Dictionary<string, object> dict,  out string exceMsg)
        {
            exceMsg = "";
            if (step.Params != null)
            { 
                foreach (FlowStepParamModel param in step.Params)
                {
                    string str = step.StepName + ">>参数" + param.ParamName;
                    object value = null;

                    if (param.IsPreStep) //如果从上一步取值
                    {
                        if (dataPre == null || !dataPre.Table.Columns.Contains(param.ParamName))
                        {
                            exceMsg = str + "无法从上一步取值值";
                            return false;
                        }

                        value = dataPre[param.ParamName]; 
                    }
                    else //从Request中取值 
                    {
                        if (dict.ContainsKey(param.ParamName))
                        {
                            value = dict[param.ParamName]; 
                        }
                        else
                        {
                            exceMsg = str + "没有在Request中获取到值";
                            return false;
                        } 
                    }

                    cmd.Parameters.AddWithValue(param.ParamName, value);
                }
            }
            return true;
        }
        private async Task InvokeException(string exception)
        {

            DataResult returnValue = new DataResult();
            returnValue.DataType = 1;
            returnValue.Exception = exception;
            this.logger.LogError(exception);
            string json = JsonConvert.SerializeObject(returnValue);
            await this.WriteAsync(json);
        }

        private async Task WriteAsync(string text)
        {
            // await this.context.Response.WriteAsync(text, Encoding.UTF8);  , Encoding.GetEncoding("GB2312")
            this.context.Response.ContentType = "text/plain;charset=utf-8";
            await this.context.Response.WriteAsync(text);
        }

    }
}
