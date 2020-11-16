using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{
    public class JmsCommand
    {
        public string DataBase { get; set; }
        public string CommandType { get; set; }
        public string CommandText { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
