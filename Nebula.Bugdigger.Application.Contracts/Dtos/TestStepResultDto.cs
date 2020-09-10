using System;
using System.Collections.Generic;

namespace Nebula.Bugdigger
{
    public class TestStepResultDto
    {
        public TestDataDto Param { get; set; } = new TestDataDto();
        public string DataDirection { get; set; }
        public string Expect { get; set; }
        public string Actual { get; set; }
        public bool IsPass { get; set; }
        public string Remark { get; set; }
        public List<ICDResultDto> ICD { get; set; } = new List<ICDResultDto>();
    }
}
