using ApiDoc.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{ 
    public class InterfaceModel : BaseModel
    { 
        public string Title { get; set; }
        public string Url { get; set; }

        private string method = "Post"; 
        public string Method
        {
            get {
                return method;
            }
            set {
                this.method = value;
            }
        }
        public int FKSN { get; set; }

        private string serializeType = "Json";
        /// <summary>
        /// // 数据格式:xml , json  
        /// </summary>
        public string SerializeType
        {
            get => serializeType;
            set => serializeType = value;
        }

        /// <summary>
        /// 结果集: Int, Scalar, DataSet
        /// </summary>
        public string ExecuteType { get; set; } 
        public bool IsTransaction { get; set; }

        public bool IsStop { get; set; } //是否停用

        /// <summary>
        /// 数据库，SqlServer, Oracle, MySql
        /// </summary>
        public string DataType { get; set; }


        public List<FlowStepModel> Steps;
        public string Auth;// 验证接口参数是否正确
    }
}
