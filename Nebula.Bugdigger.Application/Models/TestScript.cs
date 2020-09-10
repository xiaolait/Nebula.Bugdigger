using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Nebula.Bugdigger
{
    [XmlType(TypeName = "Config")]
    public class TestScript
    {
        public List<UseCase> UseCases { get; set; } = new List<UseCase>();
    }
}
