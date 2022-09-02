using Amazon.AppConfigData;
using Microsoft.Extensions.DependencyInjection;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.FeatureManager;
using System.Diagnostics.CodeAnalysis;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.IoC
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IFeatureToggleManagementBuilder UseAwsFeatureManagement(this IServiceCollection services)
        {
            services.AddScoped<IFeatureToggleProvider, AwsFeatureToggleProvider>();
            services.AddMemoryCache();
            services.AddScoped<IFeatureToggleManager, AwsFeatureManager>();
            services.AddScoped<IAmazonAppConfigData, AmazonAppConfigDataClient>();
            return new AwsFeatureManagerBuilder(services);
        }
    }
}
