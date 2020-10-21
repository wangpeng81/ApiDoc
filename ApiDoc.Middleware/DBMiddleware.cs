﻿using ApiDoc.IDAL;
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

namespace ApiDoc.Middleware
{
    public class DBMiddleware
    {

        private string SqlConnStr;

        private readonly RequestDelegate next;
        private readonly IDbHelper dbHelp;
        private readonly DBRouteValueDictionary routeDict;
        private readonly ILogger<DBMiddleware> logger;
        private HttpContext context;

        private string ServerIP = "";
        private string pwd = "";

        public DBMiddleware(RequestDelegate _next,
                            IInterfaceDAL interfaceDAL,
                            IFlowStepDAL flowStpeDAL,
                            IDbHelper dbHelp,
                            DBRouteValueDictionary _routeDict,
                            IConfiguration config,
                            ILogger<DBMiddleware> logger)
        {
            this.next = _next;
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
                dbInter.Steps = flowStpeDAL.Query(SN);
                dbInter.IsTransaction = model.IsTransaction;
                dbInter.ExecuteType = model.ExecuteType;
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
                    string text = this.InvokeFistStep(response, connection, tran, out exceMsg);

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

        private string InvokeFistStep(DBInterfaceModel response, SqlConnection connection, SqlTransaction tran, out string exceMsg)
        {
            exceMsg = "";

            FlowStepModel flow = response.Steps[0];
            if (flow.CommandText == "")
            {
                exceMsg = flow.StepName + " 没有数据库语句，请维护";
                return "";
            }

            connection.ChangeDatabase(flow.DataBase);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            cmd.Connection = connection;
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = cmd;
            cmd.Transaction = tran;

            //初始化参数
            switch (flow.CommandType)
            {
                case "Text":
                    cmd.CommandType = CommandType.Text;
                    break;
                case "StoredProcedure":
                    cmd.CommandType = CommandType.StoredProcedure;
                    break;
            }

            cmd.CommandText = flow.CommandText;
            cmd.Parameters.Clear();
            if (response.Method.ToLower() == "get")
            {
                foreach (string key in context.Request.Query.Keys)
                {
                    StringValues value = new StringValues();
                    bool value1 = context.Request.Query.TryGetValue(key, out value);
                    cmd.Parameters.AddWithValue(key, value[0]);
                }
            }
            else if (response.Method.ToLower() == "post")
            {
                var reader = new StreamReader(context.Request.Body);
                var contentFromBody = reader.ReadToEnd();
                if (contentFromBody != "")
                {
                    Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentFromBody);
                    foreach (KeyValuePair<string, object> kv in dict)
                    {
                        cmd.Parameters.AddWithValue(kv.Key, kv.Value);
                    }
                }
            }

            //执行Sql
            string result = this.ExecSql(response, connection, sqlDA, cmd);
            this.InvokeOtherStep(response, connection, tran);

            return result;
        }

        private string InvokeOtherStep(DBInterfaceModel response, SqlConnection connection, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            cmd.Connection = connection;
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = cmd;
            cmd.Transaction = tran;

            string exceMsg = "";
            for (int i = 1; i < response.Steps.Count; i++)
            {
                FlowStepModel flow = response.Steps[i];
                if (flow.CommandText == "")
                {
                    exceMsg = flow.StepName + " 没有数据库语句，请维护";
                    return exceMsg;
                }

                connection.ChangeDatabase(flow.DataBase);
                switch (flow.CommandType)
                {
                    case "Text":
                        cmd.CommandType = CommandType.Text;
                        break;
                    case "StoredProcedure":
                        cmd.CommandType = CommandType.StoredProcedure;
                        break;
                }

                cmd.CommandText = flow.CommandText;
                cmd.Parameters.Clear();

                if (response.Method.ToLower() == "get")
                {
                    foreach (string key in context.Request.Query.Keys)
                    {
                        StringValues value = new StringValues();
                        bool value1 = context.Request.Query.TryGetValue(key, out value);
                        cmd.Parameters.AddWithValue(key, value[0]);
                    }
                }
                else if (response.Method.ToLower() == "post")
                {
                    try
                    {
                        var reader = new StreamReader(context.Request.Body);
                        var contentFromBody = reader.ReadToEnd();
                        if (contentFromBody != "")
                        {
                            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentFromBody);
                            foreach (KeyValuePair<string, object> kv in dict)
                            {
                                cmd.Parameters.AddWithValue(kv.Key, kv.Value);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return exceMsg;
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
