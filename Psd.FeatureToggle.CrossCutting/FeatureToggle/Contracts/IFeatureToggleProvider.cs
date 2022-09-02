using Psd.FeatureToggle.CrossCutting.FeatureToggle.Models;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts
{
    public interface IFeatureToggleProvider
    {
        IAsyncEnumerable<FeatureToggleDefinition> GetAllFeatureTogglesAsync(bool forceReload = false);

        Task<FeatureToggleDefinition> GetFeatureToggleAsync(string name, bool forceReload = false);
    }
}
