using System;
using System.Threading.Tasks;
using Volo.Abp;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.Bugdigger.ConsoleHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string inputPath = args[0];
            string outputPath = args[1];

            using (var application = AbpApplicationFactory.Create<BugdiggerConsoleHostModule>(option => {
                option.UseAutofac();
            }))
            {
                application.Initialize();
                var xmlService = application.ServiceProvider.GetService<XmlService>();
                var testExecAppService = application.ServiceProvider.GetService<ITestExecAppService>();

                var project = xmlService.ReadStr<Project>(File.ReadAllText(inputPath));
                var testResults = new List<TestResultDto>();
                foreach (var testflow in project.Testflow)
                {
                    foreach (var step in testflow.step)
                    {
                        for (int i = 0; i < step.exectimes; i++)
                        {
                            if (step.delaytime > 0)
                            {
                                await Task.Delay(step.delaytime);
                            }
                            string configpath = "";
                            if (step.xmlPath != "")
                            {
                                configpath = step.xmlPath + "/" + step.xmlname;
                                Console.WriteLine(configpath);
                            }
                            else
                            {
                                int index = inputPath.LastIndexOf("/");
                                configpath = inputPath.Substring(0, index + 1) + "/" + step.xmlname;
                                Console.WriteLine(configpath);
                            }
                            string text = File.ReadAllText(configpath);
                            var testCaseDto = new TestCaseDto()
                            {
                                Name = step.xmlname,
                                Script = text
                            };
                            var testResult = await testExecAppService.CreateAsync(testCaseDto);
                            if (testResult != null) testResults.AddRange(testResult);

                        }
                    }
                }
                if (testResults.Count == 0)
                {
                    Console.WriteLine("No result!");
                    return;
                }

                File.WriteAllText(outputPath, JsonConvert.SerializeObject(testResults));

                Console.WriteLine("Auto Test Finish!");
            }
        }
    }
}
