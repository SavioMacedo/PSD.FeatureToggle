using Microsoft.Extensions.DependencyInjection;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts
{
    public interface IFeatureToggleManagementBuilder
    {
        IServiceCollection Services { get; }

        IFeatureToggleManagementBuilder AddFeatureFilter<T>() where T : IFeatureToggleFilter;
    }
}
