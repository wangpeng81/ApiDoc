 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    [Table("api_flow_step")]
    public class FlowStepModel : BaseModel
    { 
        public string StepName { get; set; }
        public int StepOrder { get; set; } 
        public int FKSN { get; set; }
        public bool IsTransaction { get; set; }

        public string CommandText { get; set; }

        //执行语句的类型
        public string CommandType { get; set; }
    }
}
