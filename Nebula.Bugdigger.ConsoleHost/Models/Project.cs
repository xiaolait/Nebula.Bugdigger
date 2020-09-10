using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nebula.Bugdigger.ConsoleHost
{
    public class Project
    {
        [XmlAttribute]
        public string ID { get; set; }
        public string ProjName { get; set; }
        public string SoftwareName { get; set; }
        public string SoftwarePath { get; set; }
        public string Version { get; set; }
        public string Creater { get; set; }
        public string CreateTime { get; set; }
        public string PlaneType { get; set; }
        [XmlElement]
        public List<Testflow> Testflow { get; set; } = new List<Testflow>();
    }
}
