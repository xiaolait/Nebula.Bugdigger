using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Nebula.Bugdigger
{
    public interface ITestExecAppService : IApplicationService
    {
        Task<List<TestResultDto>> CreateAsync(TestCaseDto testCase);
    }
}
