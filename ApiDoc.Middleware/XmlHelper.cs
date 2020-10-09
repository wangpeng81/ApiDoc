using ApiDoc.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ApiDoc.Middleware
{
    public class XmlHelper
    {
        public string SerializeXML(object value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(object));
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            serializer.Serialize(writer, value);
            string xml = writer.ToString();
            writer.Close();
            writer.Dispose();
            return xml;
        }

        public string SerializeXML(DataSet value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataSet));
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            serializer.Serialize(writer, value);
            string xml = writer.ToString();
            writer.Close();
            writer.Dispose();
            return xml;
        }

    }
}
