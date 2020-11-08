using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
    public class MyConfig
    {
        /// <summary>
        /// 验证码
        /// </summary> 
        public JWTTokenOptions JWTTokenOptions { get; set; }
        public DataBases DataType { get; set; }

        public DataBase this[string dataType]
        {
            get
            {
                DataBase dataBase = null;
                switch (dataType)
                {
                    case "SqlServer":
                        dataBase = this.DataType.SqlServer;
                        break;
                    case "MySql":
                        dataBase = this.DataType.MySql;
                        break;
                    case "Oracle":
                        dataBase = this.DataType.Oracle;
                        break;
                }
                return dataBase;
            }
        }
    }
}
