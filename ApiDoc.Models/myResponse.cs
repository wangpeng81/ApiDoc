using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models
{
    public class myResponse
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

        private object result;
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

    }
}
