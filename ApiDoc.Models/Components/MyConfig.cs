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
        public string Authorize { get; set; }
        public DataBases DataBases { get; set; }

        public DataBase this[string dataBase]
        {
            get
            {
                DataBase dataBase1 = null;
                switch (dataBase)
                {
                    case "SqlServer":
                        dataBase1 = this.DataBases.SqlServer;
                        break;
                    case "MySql":
                        dataBase1 = this.DataBases.MySql;
                        break;
                    case "Oracle":
                        dataBase1 = this.DataBases.Oracle;
                        break;
                }
                return dataBase1;
            }
        }
    }
}
