using ApiDoc.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    [Table("api_interface")]
    public class InterfaceModel : BaseModel
    { 
        public string Title { get; set; }
        public string Url { get; set; }
        public string Method { get; set; } //Get,Post
        public int FKSN { get; set; } 
        public string SerializeType { get; set; }    // 数据格式:xml , json 
        public string ExecuteType { get; set; } //结果集: Int, Scalar, DataSet
        public bool IsTransaction { get; set; }

    }
}
