using System;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    public class FieldParam
    {
        [XmlAttribute]
        public int StartByte { get; set; }
        [XmlAttribute]
        public int StartBit { get; set; }
        [XmlAttribute]
        public int Length { get; set; }
    }
}
