using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Nebula.Bugdigger
{
    [DependsOn(typeof(BugdiggerApplicationContractsModule))]
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class BugdiggerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<BugdiggerApplicationAutoMapperProfile>(validate: true);
            });

            context.Services.AddScoped<ConnectionMultiplexer>(serviceProvider => {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis"]);
                return redis;
            });
        }
    }
}
