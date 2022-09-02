using Microsoft.Extensions.DependencyInjection;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.FeatureManager
{
    [ExcludeFromCodeCoverage]
    public class AwsFeatureManagerBuilder : IFeatureToggleManagementBuilder
    {
        public AwsFeatureManagerBuilder(IServiceCollection services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        public IServiceCollection Services { get; }

        public IFeatureToggleManagementBuilder AddFeatureFilter<T>() where T : IFeatureToggleFilter
        {
            Type serviceType = typeof(IFeatureToggleFilter);
            Type implementationType = typeof(T);
            IEnumerable<Type> featureFilterImplementations = implementationType.GetInterfaces()
                .Where(i => i == typeof(IFeatureToggleFilter));

            if (featureFilterImplementations.Count() > 1)
            {
                throw new ArgumentException($"Uma única Feature Toggle não pode implementar mais de uma interface de filtro.", nameof(T));
            }

            if (!Services.Any(descriptor => descriptor.ServiceType == serviceType && descriptor.ImplementationType == implementationType))
            {
                Services.AddSingleton(typeof(IFeatureToggleFilter), typeof(T));
            }

            return this;
        }
    }
}
