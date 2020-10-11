using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApiDoc.Models
{
    /// <summary>
    /// 步骤历史记录
    /// </summary>
    [Table("api_flow_step_his")]
    public class FlowStepHisModel : BaseModel
    {
        public int FKSN { get; set; }
        public string FileName { get; set; }//文件名称
        public string Text { get; set; }    //Sql内容
        public bool IsEnable { get; set; }  //是否启用 1:启用 0:停用
        public DateTime DTime { get; set; } //上传日期
        
    }
}
