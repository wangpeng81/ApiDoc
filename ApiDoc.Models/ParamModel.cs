using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{

    public class ParamModel : BaseModel
    {
        public int FKSN { get; set; }
        public string ParamName { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; } //默认值
        public string Remark { get; set; }  //备注
        
    }
}
