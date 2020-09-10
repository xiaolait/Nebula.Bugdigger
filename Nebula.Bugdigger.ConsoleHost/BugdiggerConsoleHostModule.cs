using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Nebula.Bugdigger.ConsoleHost
{
    [DependsOn(typeof(AbpAutofacModule))]
    [DependsOn(typeof(BugdiggerApplicationModule))]
    public class BugdiggerConsoleHostModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<ConfigurationBuilder>(options => {
                options.AddJsonFile("appsettings.json").Build();
            });
        }
    }
}
