using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
   public class DataBase
    {
        public string ApiDocConnStr { get; set; } 
        public List<string> DataBases { get; set; }
    }
}
