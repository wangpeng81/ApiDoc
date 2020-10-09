using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ApiDoc.Models.Responses
{
    [Serializable]
     public class XmlDataResult
    {
        private int dataType = 0;
        [XmlAttribute]
        public int DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        private string exception = "";
        [XmlAttribute]
        public string Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        private object result;
        [XmlAttribute]
        public object Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
