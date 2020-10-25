using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models
{
    public class FlowStepParamModel : ParamModel
    {
        public bool IsPreStep { get; set; } //是否从前一步取值
    }
}
