using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nebula.Bugdigger.ConsoleHost
{
    public class Testflow
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlElement]
        public List<step> step { get; set; } = new List<step>();
    }
}
