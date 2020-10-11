using ApiDoc.IDAL;
using ApiDoc.Models;
using ApiDoc.Models.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;


namespace ApiDoc.DAL
{
    public class BaseDAL : IBaseDAL
    {
        protected string tableName = "";
        protected readonly ILogger<BaseDAL> _logger;
        protected readonly IDbHelper db;
 
        public BaseDAL(ILogger<BaseDAL> logger, IDbHelper db)
        {
            _logger = logger;
            this.db = db;
        }

        public int Delete(BaseModel model)
        {
            string tableName = this.GetTabeName(model);
            string cmdText = "delete from " + tableName + " where SN =" + model.SN.ToString(); 
            int iResult = db.ExecuteSql(cmdText);
            return iResult;
        }

        public BaseModel Get(BaseModel model)
        {
            if (model.SN > 0)
            {
                Type T = model.GetType();
                string tableName = this.GetTabeName(model);
                string cmdText = "select * from " + tableName + " where SN=" + model.SN.ToString(); 
                DataTable dt = db.FillTable(cmdText);
                if (dt.Rows.Count > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    PropertyInfo[] propertys = T.GetProperties();
                    foreach (PropertyInfo pro in propertys)
                    {
                        if (dt.Columns.Contains(pro.Name))
                        {
                            object value = dataRow[pro.Name];
                            pro.SetValue(model, value);
                        }
                    }
                }
                return model;
            }
            return new BaseModel();
        }

        public int Insert(BaseModel model)
        {
            Type T = model.GetType(); 
            string tableName = this.GetTabeName(model); 
            StringBuilder sql = new StringBuilder("insert into ");
            sql.Append(tableName); 
            sql.Append(" (");

            //循环对象的属性名：取得列名
            PropertyInfo[] propertys = T.GetProperties();

            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
             
            string strIdentityName = "";
            foreach (PropertyInfo pro in propertys)
            { 
                //取得是否有自动增长的特性 
                IdentityAttribute att = Attribute.GetCustomAttribute(pro, typeof(IdentityAttribute)) as IdentityAttribute;
                if (att == null || !att.IsIdentity)
                {
                    if (sbColumns.Length > 0)
                    {
                        sbColumns.Append(",");
                        sbValues.Append(",");
                    }
                    sbColumns.Append(pro.Name);
                    sbValues.Append("@" + pro.Name);
                }
                else
                {
                    strIdentityName = pro.Name;
                }
            }
               
            sql.Append(sbColumns.ToString());
            sql.Append(") values(" + sbValues.ToString() + ")");

            if (strIdentityName != "")
            {
                sql.Append("; select @@identity"); 
            }
            //循环取出对象的属性值:为列赋值 
            DbParameters paras = new DbParameters();
            foreach (PropertyInfo pro in propertys)
            {
                //取得是否有自动增长的特性 
                if (pro.Name != strIdentityName)
                {
                    object value = pro.GetValue(model); 
                    paras.Add(pro.Name, value);
                } 
            } 
            object result = db.ExecuteScalar(sql.ToString(), paras);

            int SN = int.Parse(result.ToString());
            return SN; 
        }

        public int Update(BaseModel model)
        {
            Type T = model.GetType();
            string tableName = this.GetTabeName(model);
            StringBuilder sql = new StringBuilder("update " + tableName + " set ");
            

            string strIdentityName = "";
            PropertyInfo[] propertys = T.GetProperties();
            StringBuilder sbColumns = new StringBuilder(); 
            foreach (PropertyInfo pro in propertys)
            { 
                //取得是否有自动增长的特性 
                IdentityAttribute att = Attribute.GetCustomAttribute(pro, typeof(IdentityAttribute)) as IdentityAttribute;
                if (att == null || !att.IsIdentity)
                {
                    if (sbColumns.Length > 0)
                    {
                        sbColumns.Append(","); 
                    }
                    sbColumns.Append(string.Format("{0} = @{0}", pro.Name)); 
                }
                else
                {
                    strIdentityName = pro.Name;
                }
            }
            sql.Append(sbColumns.ToString());
            sql.Append( " where " + strIdentityName + "=" + model.SN.ToString());

          
            DbParameters paras = new DbParameters();
            foreach (PropertyInfo pro in propertys)
            { 
                object value = pro.GetValue(model);
                paras.Add(pro.Name, value); 
            } 
            int iResult = db.ExecuteSql(sql.ToString(), paras);
            return iResult;
        }

        protected void CreateModel(BaseModel model, DataRow dataRow)
        {
            Type T = model.GetType(); 
            PropertyInfo[] propertys = T.GetProperties();
            DataTable dt = dataRow.Table;
            foreach (PropertyInfo pro in propertys)
            {
                if (dt.Columns.Contains(pro.Name))
                {
                    object value = dataRow[pro.Name];
                    pro.SetValue(model, value);
                }
            }
        }
 
        private string GetTabeName(BaseModel model)
        {
            Type T = model.GetType();
            TableAttribute tableAttribute = Attribute.GetCustomAttribute(T, typeof(TableAttribute)) as TableAttribute;
            return tableAttribute.Name;
        }

        protected string GetTable(Type T)
        { 
            TableAttribute tableAttribute = Attribute.GetCustomAttribute(T, typeof(TableAttribute)) as TableAttribute;
            return tableAttribute.Name;
        }
    }
}
 