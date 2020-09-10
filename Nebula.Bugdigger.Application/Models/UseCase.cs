using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    public class UseCase
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlElement]
        public List<TestStep> TestStep { get; set; } = new List<TestStep>();
    }
}
