using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Reflection;
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

            string excCmdText = ""; //存储异常的存储过程名

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

                    if (excCmdText != "")
                    {
                        excCmdText += ",";
                    }
                    excCmdText += flow.CommandText;
                     
                    if (flow.CommandText == "")
                    { 
                        string msg1 = flow.StepName + " 没有数据库语句，请维护";
                        await InvokeException(msg1);
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
                        try
                        {
                            var reader = new StreamReader(context.Request.Body);
                            var contentFromBody = reader.ReadToEnd();
                            if (contentFromBody != "")
                            {
                                Dictionary<string, object> dict = JsonHelper.DeserializeJSON<Dictionary<string, object>>(contentFromBody);
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
                #endregion

                //执行语句　
                await ExecSql(response, tran, sqlDA, cmd, excCmdText);　　
            }
            else
            {
                await context.Response.WriteAsync(path + "没有任何步骤，请维护");
            }
        }

        private async Task ExecSql(DBInterfaceModel response, SqlTransaction tran, SqlDataAdapter sqlDA, SqlCommand cmd, string excCmdText)
        {
            string json = "";
            try
            {
                XmlHelper xmlHelp = new XmlHelper();

                switch (response.ExecuteType)
                {
                    case "Scalar":
                        object obj = cmd.ExecuteScalar();
                        //DataResult dataResult = new DataResult(); 
                        //dataResult.Result = obj;
                        if (response.SerializeType == "Xml")
                        {
                            json = xmlHelp.SerializeXML(obj);
                        }
                        else
                        {
                            json = JsonHelper.SerializeJSON(obj);
                        } 
                        break;
                    case "Int":
                        int iResult = cmd.ExecuteNonQuery();
                        //IntDataResult intdataResult = new IntDataResult(); 
                        //intdataResult.Result = iResult; 
                        if (response.SerializeType == "Xml")
                        {
                            json = xmlHelp.SerializeXML(iResult);
                        }
                        else
                        {
                            json = JsonHelper.SerializeJSON(iResult); 
                        }

                        break;
                    case "DataSet":
                       
                        DataSet ds = new DataSet();
                        sqlDA.Fill(ds);
                     
                        if (response.SerializeType == "Xml")
                        {
                            XmlDSDataResult xmlResult = new XmlDSDataResult(); 
                            json = xmlHelp.SerializeXML(ds);
                        }
                        else
                        {
                            //DSDataResult dsDataResult = new DSDataResult();
                            //dsDataResult.Result = ds;
                            json = JsonHelper.SerializeJSON(ds); 
                        } 
                        break;
                }
            }
            catch (System.Exception ex)
            {
                string msg = excCmdText + ex.Message; 
                await InvokeException(msg);
                return ;
            }

            if (response.IsTransaction)
            {
                tran.Commit();
            }

            await context.Response.WriteAsync(json); 
        }
        private async Task InvokeException(string exception) {

            DataResult returnValue = new DataResult();
            returnValue.DataType = 1;
            returnValue.Exception = exception;
            string json = JsonHelper.SerializeJSON<DataResult>(returnValue);
            await this.context.Response.WriteAsync(json);
        }
    }
}
