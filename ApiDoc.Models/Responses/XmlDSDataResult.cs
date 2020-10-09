using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml.Serialization;

namespace ApiDoc.Models.Responses
{
    [Serializable]
    public class XmlDSDataResult : XmlDataResult
    {
        private DataSet result;
        [XmlAttribute]
        public new DataSet Result
        {
            get { return result; }
            set { result = value; }
        }
    }
}
