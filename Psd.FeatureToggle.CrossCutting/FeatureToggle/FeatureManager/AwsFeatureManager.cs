using Psd.FeatureToggle.CrossCutting.FeatureToggle.Attributes;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Constants;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Models;
using System.Text.Json;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.FeatureManager
{
    public class AwsFeatureManager : IFeatureToggleManager
    {
        private readonly IFeatureToggleProvider _featureDefinitionProvider;
        private readonly IEnumerable<IFeatureToggleFilter> _filters;
        public AwsFeatureManager(IFeatureToggleProvider featureDefinitionProvider, IEnumerable<IFeatureToggleFilter> filters)
        {
            _featureDefinitionProvider = featureDefinitionProvider;
            _filters = filters;
        }

        public async IAsyncEnumerable<string> GetFeatureNamesAsync()
        {
            await foreach (FeatureToggleDefinition item in _featureDefinitionProvider.GetAllFeatureTogglesAsync())
                yield return item.Name;
        }
        public async Task<bool> IsEnabledAsync(string feature)
        {
            FeatureToggleDefinition featureDefinition = await _featureDefinitionProvider.GetFeatureToggleAsync(feature);
            return featureDefinition.IsEnabled;
        }
        public async Task<bool> IsEnabledAsync(string feature, string filterName)
        {
            FeatureToggleDefinition featureDefinition = await _featureDefinitionProvider.GetFeatureToggleAsync(feature);
            if (!featureDefinition.IsEnabled)
                return false;

            IEnumerable<IFeatureToggleFilter> matchingFilters = _filters.Where(f =>
            {
                Type filterType = f.GetType();
                string name = ((FeatureFilterAliasAttribute)Attribute.GetCustomAttribute(filterType, typeof(FeatureFilterAliasAttribute)))?.Alias;

                name ??= filterType.Name;

                return name.Equals(filterName, StringComparison.OrdinalIgnoreCase);
            });

            if (matchingFilters.Count() > 1)
                throw new Exception($"Existem vários FeatureFilters corresponde a configuração de filtro para '{filterName}'.");

            IFeatureToggleFilter filter = matchingFilters.FirstOrDefault();

            return filter is null ? false : await filter.EvaluateAsync(featureDefinition, (IFeatureToggleContext)null);
        }

        public async Task<bool> IsEnabledByUserNameAsync(string feature, string userName)
        {
            return await IsEnabled(feature, userName, FeatureToggleConstants.UsersNameProperty);
        }
        public async Task<bool> IsEnabledByRoleAsync(string feature, string role)
        {
            return await IsEnabled(feature, role, FeatureToggleConstants.RolesProperty);
        }
        private async Task<bool> IsEnabled(string feature, string value, string property)
        {
            FeatureToggleDefinition featureDefinition = await _featureDefinitionProvider.GetFeatureToggleAsync(feature);
            if (!featureDefinition.IsEnabled)
                return false;
            List<string> users = JsonSerializer.Deserialize<List<string>>(featureDefinition.Parameters.GetValueOrDefault(property));
            return users.Contains(value);
        }

        public async Task<bool> IsEnabledAsync<T>(string feature, string filterName, T toggleContext) where T : IFeatureToggleContext
        {
            FeatureToggleDefinition featureDefinition = await _featureDefinitionProvider.GetFeatureToggleAsync(feature);
            if (!featureDefinition.IsEnabled)
                return false;

            IEnumerable<IFeatureToggleFilter> matchingFilters = _filters.Where(f =>
            {
                Type filterType = f.GetType();
                string name = ((FeatureFilterAliasAttribute)Attribute.GetCustomAttribute(filterType, typeof(FeatureFilterAliasAttribute)))?.Alias;

                name ??= filterType.Name;

                return name.Equals(filterName, StringComparison.OrdinalIgnoreCase);
            });

            if (matchingFilters.Count() > 1)
                throw new Exception($"Existem vários FeatureFilters corresponde a configuração de filtro para '{filterName}'.");

            IFeatureToggleFilter filter = matchingFilters.FirstOrDefault();

            return filter is null ? false : await filter.EvaluateAsync(featureDefinition, toggleContext);
        }
    }
}
