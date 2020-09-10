using System;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    public enum BusType
    {
        [XmlEnum(Name = "1553")]
        P_1553B = 0,
        [XmlEnum(Name = "422")]
        P_ARINC422 = 3,
        [XmlEnum(Name = "429")]
        P_ARINC429 = 1,
        [XmlEnum(Name = "FC")]
        P_FC = 4,
        [XmlEnum(Name = "IO")]
        P_IO = 5,

    }
}
