using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ApiDoc.IDAL
{
    public class DbParameters
    {
        private List<SqlParameter> li;

        //构造函数
        public DbParameters()
        {
            li = new List<SqlParameter>();
        }

        //单个参数的构造函数
        public DbParameters(string strName, object strValue)
        {
            li = new List<SqlParameter>();
            this.Add(strName, strValue);
        }

        //长度
        public int Length
        {
            get { return li.Count; }
        }
        //索引
        public SqlParameter this[int k]
        {
            get
            {
                if (li.Contains(li[k]))
                {
                    SqlParameter parm = li[k];
                    return parm;
                }
                else
                {
                    return null;
                }
            }
        }

        //添加 Input 类型参数
        public void Add(string sName, object sValue)
        {
            li.Add(new SqlParameter()

            {
                ParameterName = sName.Trim(),
                Value = sValue ?? DBNull.Value,
                Direction = ParameterDirection.Input,
            });
        }

        public void AddOut()
        {
            AddOut("@Result", "int", 4);
        }
        public void AddOut(string sName, string sDbType, int iSize)
        {
            li.Add(new SqlParameter()

            {
                ParameterName = sName,
                SqlDbType = ConvertSqlDbType(sDbType),
                Size = iSize,
                Direction = ParameterDirection.Output,
            });
        }
        public void AddInputOutput(string sName)
        {
            li.Add(new SqlParameter()

            {
                ParameterName = sName,
                Direction = ParameterDirection.InputOutput,
            });
        }
        public void AddInputOutput(string sName, string sDbType, int iSize)
        {
            li.Add(new SqlParameter()

            {
                ParameterName = sName,
                SqlDbType = ConvertSqlDbType(sDbType),
                Size = iSize,
                Direction = ParameterDirection.InputOutput,
            });
        }

        public void Output()
        {
            //netcore2.0里没有HttpContext后续这里改为日志记录
            //System.Web.HttpContext.Current.Response.Write("参数输出：---- <br />");

            for (int i = 0; i < li.Count; i++)
            {
                SqlParameter p = li[i];
                string pName = p.ParameterName;
                string pVal = Convert.ToString(p.Value);
                //System.Web.HttpContext.Current.Response.Write(pName + " 的值为： " + pVal + " <br />");
            }
        }

        private SqlDbType ConvertSqlDbType(string strDbType)
        {
            SqlDbType t = new SqlDbType();
            switch (strDbType.Trim().ToLower())
            {
                case "nvarchar": t = SqlDbType.NVarChar; break;
                case "nchar": t = SqlDbType.NChar; break;
                case "varchar": t = SqlDbType.VarChar; break;
                case "char": t = SqlDbType.Char; break;
                case "int": t = SqlDbType.Int; break;
                case "datetime": t = SqlDbType.DateTime; break;
                case "decimal": t = SqlDbType.Decimal; break;
                case "bit": t = SqlDbType.Bit; break;
                case "text": t = SqlDbType.Text; break;
                case "ntext": t = SqlDbType.NText; break;
                case "money": t = SqlDbType.Money; break;
                case "float": t = SqlDbType.Float; break;
                case "binary": t = SqlDbType.Binary; break;
            }
            return t;
        }

        public void Clear()
        {
            li.Clear();
        }

    }
}