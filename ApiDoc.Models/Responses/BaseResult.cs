using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Responses
{
    public class BaseResult 
    {
        private int dataType = 0;
        public int DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        private string exception = "";
        public string Exception
        {
            get { return exception; }
            set { exception = value; }
        }

       
    }
}
