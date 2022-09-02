namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.FeatureManager
{
    public class AwsFeatureManagerConfiguration
    {
        public string ApplicationIdentifier { get; set; }
        public string ConfigurationProfileIdentifier { get; set; }
        public string EnvironmentIdentifier { get; set; }
        public int RequiredMinimumPollIntervalInSeconds { get; set; }
        public bool UseCache { get; set; }
        public int ExpireFeatureCacheInMinutes { get; set; }
    }
}
