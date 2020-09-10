using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Nebula.Bugdigger
{
    public class TestExecAppService : ApplicationService, ITestExecAppService
    {
        private readonly TestExecService _testExecService;
        private readonly XmlService _xmlService;

        public TestExecAppService(TestExecService testExecService, XmlService xmlService)
        {
            _testExecService = testExecService;
            _xmlService = xmlService;
        }

        public async Task<List<TestResultDto>> CreateAsync(TestCaseDto testCase)
        {
            var testResults = new List<TestResultDto>();
            var testCaseConfig = _xmlService.ReadStr<TestScript>(testCase.Script);

            if (testCaseConfig == null) return null;
            foreach (var usecase in testCaseConfig.UseCases)
            {
                var testResult = await _testExecService.RunTestScript(usecase);
                testResult.TestCaseId = testCase.Id ?? testResult.TestCaseId;
                testResult.TestCaseName = testCase.Name;
                testResults.Add(testResult);
            }
            return testResults;
        }
    }
}
