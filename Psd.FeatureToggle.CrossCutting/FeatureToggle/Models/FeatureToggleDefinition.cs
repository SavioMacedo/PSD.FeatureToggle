using Newtonsoft.Json;

namespace Psd.FeatureToggle.CrossCutting.FeatureToggle.Models
{
    public class FeatureToggleDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enabled")]
        public bool IsEnabled { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public static class FeatureToggleDefinitionExtensions
    {
        public static T GetPropertyValue<T>(this Dictionary<string, string> dict, string property)
        {
            if (!dict.ContainsKey(property))
                return default(T);
            string json = dict[property];
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
