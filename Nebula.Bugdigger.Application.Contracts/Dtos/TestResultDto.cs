using System.Collections.Generic;

namespace Nebula.Bugdigger
{
    public class TestResultDto
    {
        public string TestCaseId { get; set; }
        public string TestCaseName { get; set; }
        public string TestTime { get; set; }
        public string Result { get; set; }
        public string IsPass { get; set; }
        public List<TestStepResultDto> TestStepResults { get; set; } = new List<TestStepResultDto>();
    }
}
