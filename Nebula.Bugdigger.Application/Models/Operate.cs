using System;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    public enum Operate
    {
        [XmlEnum(Name = "Send")]
        Send,
        [XmlEnum(Name = "Recv")]
        Recv
    }
}
