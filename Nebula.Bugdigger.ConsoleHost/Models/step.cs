using System;
using System.Xml.Serialization;

namespace Nebula.Bugdigger.ConsoleHost
{
    public class step
    {
        [XmlAttribute]
        public string Index { get; set; }

        public string xmlPath { get; set; }
        public string xmlname { get; set; }
        public int delaytime { get; set; }
        public int exectimes { get; set; }
    }
}
