using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Volo.Abp.DependencyInjection;

namespace Nebula.Bugdigger
{
    public class XmlService : ITransientDependency
    {
        public T ReadStr<T>(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer xz = new XmlSerializer(typeof(T));
            object ob = null;
            try
            {
                ob = xz.Deserialize(reader);
            }
            catch
            {
                return (T)ob;
            }

            reader.Close();
            return (T)ob;
        }

        public string WriteStr(object xmlObject)
        {
            XmlDocument xd = new XmlDocument();
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(xmlObject.GetType());
                xz.Serialize(sw, xmlObject);
                return sw.ToString();
            }
        }
    }
}
