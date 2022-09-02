using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Contracts;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.Models;
using System.Text;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.FeatureManager
{
    public class AwsFeatureToggleProvider : IFeatureToggleProvider
    {
        private readonly AwsFeatureManagerConfiguration _configuration;
        private readonly IAmazonAppConfigData _amazonAppConfigClient;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey;

        private Dictionary<string, string> _featureDefinitions;

        public AwsFeatureToggleProvider(IConfiguration configuration,
            IAmazonAppConfigData amazonAppConfigClient,
            IMemoryCache memoryCache)
        {
            _configuration = configuration.GetSection(nameof(AwsFeatureManagerConfiguration)).Get<AwsFeatureManagerConfiguration>();
            if (_configuration == null)
                throw new ArgumentNullException(nameof(configuration), "AwsAppConfig não foi parametrizado");
            _amazonAppConfigClient = amazonAppConfigClient;
            _memoryCache = memoryCache;
            _cacheKey = $"{_configuration.ApplicationIdentifier}-{_configuration.ConfigurationProfileIdentifier}-{_configuration.EnvironmentIdentifier}";
        }

        public async IAsyncEnumerable<FeatureToggleDefinition> GetAllFeatureTogglesAsync(bool forceReload = false)
        {
            if (_featureDefinitions == null || forceReload)
                await ReloadFeatures();
            foreach (KeyValuePair<string, string> featureDefinition in _featureDefinitions)
                yield return ConvertToFeatureToggleDefinition(featureDefinition.Key, featureDefinition.Value);
        }
        public async Task<FeatureToggleDefinition> GetFeatureToggleAsync(string name, bool forceReload)
        {
            if (_featureDefinitions == null || forceReload)
                await ReloadFeatures();
            if (_featureDefinitions.ContainsKey(name))
                return ConvertToFeatureToggleDefinition(name, _featureDefinitions[name]);
            return default;
        }

        private async Task ReloadFeatures()
        {
            string settingsString = _configuration.UseCache ? await _memoryCache.GetOrCreate(_cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_configuration.ExpireFeatureCacheInMinutes);
                entry.SetPriority(CacheItemPriority.High);
                return await GetFeatures();
            }) : await GetFeatures();
            ConvertJObjectToFeaturesDefinitions(JObject.Parse(settingsString));
        }

        private async Task<string> GetFeatures()
        {
            var request = new StartConfigurationSessionRequest()
            {
                ApplicationIdentifier = _configuration.ApplicationIdentifier,
                ConfigurationProfileIdentifier = _configuration.ConfigurationProfileIdentifier,
                EnvironmentIdentifier = _configuration.EnvironmentIdentifier,
                RequiredMinimumPollIntervalInSeconds = _configuration.RequiredMinimumPollIntervalInSeconds
            };
            StartConfigurationSessionResponse startSession = await _amazonAppConfigClient.StartConfigurationSessionAsync(request);
            GetLatestConfigurationResponse lastConfiguration = await _amazonAppConfigClient.GetLatestConfigurationAsync(new GetLatestConfigurationRequest()
            {
                ConfigurationToken = startSession.InitialConfigurationToken
            });
            return Encoding.Default.GetString(lastConfiguration.Configuration.ToArray());
        }

        private void ConvertJObjectToFeaturesDefinitions(JObject jobject)
        {
            _featureDefinitions = new Dictionary<string, string>();
            foreach (KeyValuePair<string, JToken> item in jobject)
                _featureDefinitions.Add(item.Key, item.Value.ToString());
        }

        private FeatureToggleDefinition ConvertToFeatureToggleDefinition(string featureName, string json)
        {
            var featureToggleDefinition = JsonConvert.DeserializeObject<FeatureToggleDefinition>(json);
            featureToggleDefinition.Name = featureName;
            foreach (KeyValuePair<string, JToken> property in JObject.Parse(json))
            {
                if (property.Key == "enabled")
                    continue;
                featureToggleDefinition.Parameters.Add(property.Key, property.Value.ToString());
            }
            return featureToggleDefinition;
        }

    }
}
