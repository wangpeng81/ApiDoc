using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic; 
using System.Data;  
using System.Net.Http;
using System.Reflection;
using System.Text;  
using System.Threading.Tasks; 
using System;
using Newtonsoft.Json; 
using Microsoft.Extensions.Logging; 
using Autofac;
using System.Data.Common;
using System.IO;
using ApiDoc.Models.Components;

namespace ApiDoc.Middleware
{
    public class DBMiddleware
    {
 
        private readonly RequestDelegate next;
        private readonly IParamDAL paramDAL;
        private readonly IDbHelper dbHelp;
        private readonly DBRouteValueDictionary routeDict;
        private readonly MyConfig myConfig;
        private readonly ILogger<DBMiddleware> logger;
        private readonly IComponentContext componentContext;
        private HttpContext context;
          
        public DBMiddleware(RequestDelegate _next,
                            IInterfaceDAL interfaceDAL,
                            IFlowStepDAL flowStepDAL,
                            IParamDAL paramDAL,
                            IDbHelper dbHelp,
                            DBRouteValueDictionary _routeDict,
                            IConfiguration config,
                            MyConfig myConfig,
                            ILogger<DBMiddleware> logger,
                            IComponentContext componentContext)
        {
            this.next = _next;
            this.paramDAL = paramDAL;
            this.dbHelp = dbHelp;
            this.routeDict = _routeDict;
            this.myConfig = myConfig;
            this.logger = logger;
            this.componentContext = componentContext;
            string SqlConnStr = config.GetConnectionString("ApiDocConnStr");  

            //加载路由集合

            List<InterfaceModel> dtInterface = interfaceDAL.Query(false);
            foreach (InterfaceModel model in dtInterface)
            {
                //加载步骤
                int SN = model.SN;
                string Url = model.Url;
                InterfaceModel dbInter = new InterfaceModel();
                dbInter.SerializeType = model.SerializeType;
                dbInter.Method = model.Method;
                dbInter.Steps = flowStepDAL.QueryOfParam(SN);
                dbInter.IsTransaction = model.IsTransaction;
                dbInter.ExecuteType = model.ExecuteType;
                dbInter.DataType = model.DataType;

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
            InterfaceModel response = this.routeDict[path];
            if (response.DataType == "" || response.DataType == null)
            {
                await this.InvokeException("没有配置数据库类型");
                return;
            }
            if (response.DataType != "SqlServer" && response.DataType != "Oracle" && response.DataType != "MySql")
            {
                await this.InvokeException("不支持数据库类型:" + response.DataType);
                return;
            }

            string exceMsg = ""; //存储异常的存储过程名  
            if (response.Steps.Count > 0)
            {
                //参数验证
                //从Request中获取参数集合
                Dictionary<string, object> dict = null;
                string method = response.Method.ToLower();
                string auth = "";
                if (method == "post" || method == "get")
                {
                    dict = this.CreateDict(method, out auth);
                } 
                else
                {
                    await this.InvokeException("此平台只支持post,get");
                    return;
                }

                auth = auth.ToLower();
                if (response.Auth != auth)
                {
                    await this.InvokeException("接收参数[" + response.Auth + "]与规则[" + auth + "]不匹配");
                    return;
                }

                IDbConnection connection = this.componentContext.ResolveNamed<IDbConnection>(response.DataType);
                string connStr = this.myConfig[response.DataType].ApiDocConnStr;
                connection.ConnectionString = connStr;
                IDbTransaction tran = null;
                try
                {
                    connection.Open();

                    if (response.IsTransaction)
                    {
                        tran = connection.BeginTransaction();
                    }

                    //执行第一步 
                    string text = this.InvokeStep(response, connection, tran, dict, out exceMsg);

                    if (exceMsg != "")
                    {
                        if (response.IsTransaction)
                        {
                            tran.Rollback();
                        }

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

        private Dictionary<string, object> CreateDict(string method, out string auth)
        {
            auth = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (method == "post")
            {
                if (context.Request.ContentType == "application/x-www-form-urlencoded")
                {
                    foreach (KeyValuePair<string, StringValues> kv in context.Request.Form)
                    {
                        if (kv.Key == "__RequestVerificationToken")
                        {
                            continue;
                        }
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

            return dict;
        }
           
        private string InvokeStep(InterfaceModel response, IDbConnection connection, IDbTransaction tran, Dictionary<string, object> dict, out string exceMsg)
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
                 
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandTimeout = 0;
                cmd.Connection = connection;

                if (response.IsTransaction)
                {
                    cmd.Transaction = tran;
                }

                IDbDataAdapter sqlDA = this.componentContext.ResolveNamed<IDbDataAdapter>(response.DataType);
                sqlDA.SelectCommand = cmd;

                //初始化参数
                switch (step.CommandType)
                {
                    case "Fun": 
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
                        DataSet ds = new DataSet();
                        sqlDA.Fill(ds); 

                        if (ds.Tables.Count > 0 && ds.Tables[ds.Tables.Count - 1].Rows.Count > 0 )
                        {
                            DataTable dt = ds.Tables[ds.Tables.Count - 1]; 
                            dataPre = dt.Rows[0];
                        }
                    }

                }
                catch (Exception ex)
                {
                    exceMsg = response.Url　+ ">>" + step.StepName + ">>" + ex.Message; 
                    this.logger.LogError(exceMsg);
                    return "";
                }
                
                rowIndex++;
            } 
            return result;
        }
 
        private string ExecSql(InterfaceModel response, IDbConnection connection, IDbDataAdapter sqlDA, IDbCommand cmd)
        {
            
            XmlHelper xmlHelp = new XmlHelper(); 
            string json = "";
            object dataResult = new object();
            object result = new object(); 
            switch (response.ExecuteType)
            {
                case "Scalar":
                    DataResult dataResult1 = new DataResult();
                    dataResult1.DataType = 200;
                    dataResult1.Result = cmd.ExecuteScalar();
                    dataResult = dataResult1;
                    result = dataResult1.Result;
                    break;
                case "Int":
                    IntDataResult intDataResult = new IntDataResult();
                    intDataResult.Result = cmd.ExecuteNonQuery();
                    intDataResult.DataType = 200;
                    dataResult = intDataResult;
                    result = intDataResult.Result;
                    break;
                case "DataSet":
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);
                    DSDataResult dSDataResult = new DSDataResult();
                    dSDataResult.Result = ds;
                    dSDataResult.DataType = 200;
                    dataResult = dSDataResult;
                    break;
            }
             
            if (response.SerializeType == "Xml")
            {
                switch (response.ExecuteType)
                {
                    case "Scalar":
                        json = xmlHelp.SerializeXML<DataResult>(dataResult);
                        break;
                    case "Int":
                        json = xmlHelp.SerializeXML<IntDataResult>(dataResult);
                        break;
                    case "DataSet":
                        json = xmlHelp.SerializeXML<DSDataResult>(dataResult);
                        break;
                }
                
            }
            else if (response.SerializeType == "Json")
            {
                json = JsonConvert.SerializeObject(dataResult);
            }
            else
            {
                json = result.ToString();
            }
            
            return json;
        }

        private bool CreateParam(InterfaceModel response, IDbCommand cmd, DataRow dataPre, FlowStepModel step, Dictionary<string, object> dict,  out string exceMsg)
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

                    DbParameter dbParameter = this.componentContext.ResolveNamed<DbParameter>(response.DataType);
                    dbParameter.ParameterName = param.ParamName;
                    dbParameter.Value = value;
                    cmd.Parameters.Add(dbParameter);
                }
            }
            return true;
        }
        
        private async Task InvokeException(string exception)
        {

            DataResult returnValue = new DataResult();
            returnValue.DataType = -1;
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
