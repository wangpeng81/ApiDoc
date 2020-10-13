using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDoc.Models
{
    [Table("api_interface_param")]
    public class ParamModel : BaseModel
    {
        public string ParamName { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Remark { get; set; }
        public int FKSN { get; set; }
    }
}
