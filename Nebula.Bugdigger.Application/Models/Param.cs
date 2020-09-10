using System;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    public class Param
    {
        [XmlAttribute]
        public BusType BusType { get; set; }
        [XmlAttribute]
        public int NetNo { get; set; }
        [XmlAttribute]
        public int SrcChannel1 { get; set; }
        [XmlAttribute]
        public int SrcChannel2 { get; set; }
        [XmlAttribute]
        public int DesChannel1 { get; set; }
        [XmlAttribute]
        public int DesChannel2 { get; set; }
        [XmlAttribute]
        public string Data { get; set; }

    }

}
