using Psd.FeatureToggle.CrossCutting.FeatureToggle.Models;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts
{
    public interface IFeatureToggleFilter
    {
        Task<bool> EvaluateAsync(FeatureToggleDefinition featureToggleDefinition, IFeatureToggleContext context, CancellationToken cancellationToken = default);
    }
}
