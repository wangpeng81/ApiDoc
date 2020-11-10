using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{
    public class LayuiResult
    {
        public int code { get; set; }
        public string  msg { get; set; }
        public int count { get; set; }
        public object data { get; set; }
    }
}
