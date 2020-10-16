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

        private string serializeType = "Json";  // 数据格式:xml , json  
        public string SerializeType
        {
            get => serializeType;
            set => serializeType = value;
        }

        public string ExecuteType { get; set; } //结果集: Int, Scalar, DataSet
        public bool IsTransaction { get; set; }
    }
}
